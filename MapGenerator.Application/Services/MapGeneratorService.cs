using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MapGenerator.Application.Services;

public class MapGeneratorService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IFeatureDefinitionProvider _featureProvider;
    private readonly IBiomeDefinitionProvider _biomeProvider;
    private HexTile[,]? _cache;
    private MapConfig? _configCache;

    public MapGeneratorService(
        IServiceScopeFactory scopeFactory,
        IFeatureDefinitionProvider featureProvider,
        IBiomeDefinitionProvider biomeProvider)
    {
        _scopeFactory    = scopeFactory;
        _featureProvider = featureProvider;
        _biomeProvider   = biomeProvider;
    }

    private IMapRepository Repo(IServiceScope scope) =>
        scope.ServiceProvider.GetRequiredService<IMapRepository>();

    // ── Public API ──────────────────────────────────────────────────────────

    public async Task<MapConfig> GenerateMapAsync(
        int width, int height, int? seed = null, MapGenerationOptions? opts = null)
    {
        opts ??= new MapGenerationOptions();
        int actualSeed = seed ?? Random.Shared.Next();

        var elevNoise  = new PerlinNoise(actualSeed);
        var moistNoise = new PerlinNoise(actualSeed + 9_973);
        var warpNoise1 = new PerlinNoise(actualSeed + 31_337);
        var warpNoise2 = new PerlinNoise(actualSeed + 7_919);

        var grid = new HexTile[width, height];

        // ── Step 1: elevation + moisture ──────────────────────────────────
        for (int r = 0; r < height; r++)
        {
            for (int q = 0; q < width; q++)
            {
                float nx = q / (float)width;
                float ny = r / (float)height;

                // Domain-warp the sampling position for organic coastlines
                float warpAmt = opts.WarpStrength * 0.28f;
                float wx = warpNoise1.Sample(nx * 4f, ny * 4f) * warpAmt;
                float wy = warpNoise2.Sample(nx * 4f + 5.2f, ny * 4f + 1.3f) * warpAmt;

                float ex = nx * opts.ElevationScale + wx;
                float ey = ny * opts.ElevationScale + wy;

                float e = Fbm(elevNoise, ex, ey, 5);
                float m = Math.Clamp(Fbm(moistNoise, nx * opts.MoistureScale, ny * opts.MoistureScale, 4) + opts.MoistureBias, 0f, 1f);

                grid[q, r] = new HexTile
                {
                    Q = q, R = r,
                    Elevation = e,
                    Moisture = m,
                    Biome = AssignRawBiome(e, m, opts)
                };
            }
        }

        // ── Step 2: flood-fill to distinguish ocean from inland lakes ─────
        ClassifyWaterBodies(grid, width, height, opts.SeaLevel);

        // ── Step 3: beach strips at water edges ───────────────────────────
        AddBeaches(grid, width, height, opts.SeaLevel);

        // ── Step 3b: shallows (shallow ocean adjacent to land) ────────────
        AddShallows(grid, width, height, opts.SeaLevel);

        // ── Step 4: rivers ────────────────────────────────────────────────
        int riverCount = opts.RiverCount > 0
            ? opts.RiverCount
            : Math.Max(5, width * height / 2_000);
        GenerateRivers(grid, width, height, actualSeed, riverCount);

        // ── Step 5: scatter tile features (overlays) ──────────────────────
        PlaceFeatures(grid, width, height, actualSeed);

        // ── Step 6: find spawn (grassland/plains near centre) ─────────────
        var flat = FlatList(grid, width, height);
        int cx = width / 2, cy = height / 2;
        var spawn = flat
            .Where(t => t.Biome is BiomeType.Grassland or BiomeType.Plains)
            .OrderBy(t => Math.Abs(t.Q - cx) + Math.Abs(t.R - cy))
            .FirstOrDefault()
            ?? flat.Where(t => t.Biome is not BiomeType.Ocean and not BiomeType.Lake
                                         and not BiomeType.Mountain and not BiomeType.Snow
                                         and not BiomeType.Glacier  and not BiomeType.Volcano)
                   .OrderBy(t => Math.Abs(t.Q - cx) + Math.Abs(t.R - cy))
                   .First();

        var config = new MapConfig
        {
            Width = width, Height = height,
            Seed = actualSeed,
            GeneratedAt = DateTime.UtcNow,
            SpawnQ = spawn.Q, SpawnR = spawn.R,
            Options = opts
        };

        // ── Persist ───────────────────────────────────────────────────────
        using (var scope = _scopeFactory.CreateScope())
        {
            var repo = Repo(scope);
            await repo.DeleteAllTilesAsync();
            await repo.SaveTilesAsync(flat);
            await repo.SaveConfigAsync(config);
        }

        // Update in-memory cache
        _cache = grid;
        _configCache = config;
        return config;
    }

    public async Task<MapConfig?> LoadCacheAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var repo = Repo(scope);

        var config = await repo.GetCurrentConfigAsync();
        if (config == null) return null;

        _configCache = config;
        _cache = new HexTile[config.Width, config.Height];

        var tiles = await repo.GetTilesInRangeAsync(0, config.Width - 1, 0, config.Height - 1);
        foreach (var t in tiles) _cache[t.Q, t.R] = t;
        return config;
    }

    public HexTile? GetCachedTile(int q, int r)
    {
        if (_cache == null) return null;
        if (q < 0 || r < 0 || q >= _cache.GetLength(0) || r >= _cache.GetLength(1)) return null;
        return _cache[q, r];
    }

    public MapConfig? GetCachedConfig() => _configCache;

    public void UpdateCachedEggCount(int q, int r, int eggCount)
    {
        var tile = GetCachedTile(q, r);
        if (tile != null) tile.EggCount = eggCount;
    }

    public void UpdateCachedSign(int q, int r, string text, string authorName)
    {
        var tile = GetCachedTile(q, r);
        if (tile != null) { tile.SignText = text; tile.SignAuthor = authorName; }
    }

    public List<HexTile> GetViewportTiles(int centerQ, int centerR, int qRadius, int rRadius)
    {
        if (_cache == null) return [];
        int w = _cache.GetLength(0), h = _cache.GetLength(1);
        var result = new List<HexTile>((qRadius * 2 + 1) * (rRadius * 2 + 1));
        for (int r = Math.Max(0, centerR - rRadius); r <= Math.Min(h - 1, centerR + rRadius); r++)
            for (int q = Math.Max(0, centerQ - qRadius); q <= Math.Min(w - 1, centerQ + qRadius); q++)
                result.Add(_cache[q, r]);
        return result;
    }

    public byte[] GetMinimapData()
    {
        if (_cache == null || _configCache == null) return [];
        int w = _configCache.Width, h = _configCache.Height;
        var data = new byte[w * h * 3];
        for (int r = 0; r < h; r++)
            for (int q = 0; q < w; q++)
            {
                var biome = _cache[q, r]?.Biome ?? BiomeType.Ocean;
                var (red, green, blue) = _biomeProvider.GetByType(biome)?.MinimapRgb ?? (28, 78, 140);
                int idx = (r * w + q) * 3;
                data[idx] = red; data[idx + 1] = green; data[idx + 2] = blue;
            }
        return data;
    }

    // ── Generation helpers ───────────────────────────────────────────────────

    /// Fractional Brownian motion — sums several octaves of Perlin noise, returns 0–1.
    private static float Fbm(PerlinNoise noise, float x, float y, int octaves)
    {
        float value = 0f, amplitude = 0.5f, frequency = 1f, total = 0f;
        for (int o = 0; o < octaves; o++)
        {
            value     += amplitude * noise.Sample(x * frequency, y * frequency);
            total     += amplitude;
            amplitude *= 0.5f;
            frequency *= 2f;
        }
        return (value / total + 1f) * 0.5f;
    }

    private static BiomeType AssignRawBiome(float e, float m, MapGenerationOptions opts)
    {
        // Water tiles — reclassified as Ocean or Lake in ClassifyWaterBodies
        if (e < opts.SeaLevel) return BiomeType.Ocean;

        // Very high peaks → Glacier
        if (e >= opts.SnowLevel + 0.04f) return BiomeType.Glacier;
        if (e >= opts.SnowLevel) return BiomeType.Snow;

        // Rare dry volcanic peaks
        if (e >= opts.MountainLevel + 0.06f && m < 0.12f) return BiomeType.Volcano;
        if (e >= opts.MountainLevel) return BiomeType.Mountain;

        float landDepth = e - opts.SeaLevel;

        // Cold sub-mountain zone with low moisture → tundra
        if (e >= opts.MountainLevel - 0.12f && m < 0.35f) return BiomeType.Tundra;

        // Near-shore moisture variants
        if (landDepth < 0.10f && m > 0.62f) return BiomeType.Swamp;
        if (landDepth < 0.12f && m >= 0.35f && m <= 0.62f) return BiomeType.Marsh;

        // Montane forest (near mountain edge, wet)
        if (e > opts.MountainLevel - 0.10f && m > 0.50f) return BiomeType.Forest;

        // Moisture gradient
        if (m > 0.78f) return BiomeType.Jungle;
        if (m > 0.62f) return BiomeType.Forest;
        if (m > 0.48f) return BiomeType.Grassland;
        if (m > 0.30f) return BiomeType.Plains;
        if (m > 0.15f) return BiomeType.Savanna;
        return BiomeType.Desert;
    }

    /// BFS from all border water tiles — anything reachable is ocean; unreachable inland water = lake.
    private static void ClassifyWaterBodies(HexTile[,] grid, int width, int height, float seaLevel)
    {
        var isOcean = new bool[width, height];
        var queue   = new Queue<(int q, int r)>();

        void TrySeed(int q, int r)
        {
            if (grid[q, r].Elevation < seaLevel && !isOcean[q, r])
            {
                isOcean[q, r] = true;
                queue.Enqueue((q, r));
            }
        }

        for (int q = 0; q < width;  q++) { TrySeed(q, 0); TrySeed(q, height - 1); }
        for (int r = 1; r < height - 1; r++) { TrySeed(0, r); TrySeed(width - 1, r); }

        while (queue.Count > 0)
        {
            var (q, r) = queue.Dequeue();
            foreach (var (dq, dr) in HexNeighborOffsets())
            {
                int nq = q + dq, nr = r + dr;
                if (nq < 0 || nq >= width || nr < 0 || nr >= height) continue;
                if (isOcean[nq, nr] || grid[nq, nr].Elevation >= seaLevel) continue;
                isOcean[nq, nr] = true;
                queue.Enqueue((nq, nr));
            }
        }

        for (int r = 0; r < height; r++)
            for (int q = 0; q < width; q++)
                if (grid[q, r].Elevation < seaLevel)
                    grid[q, r].Biome = isOcean[q, r] ? BiomeType.Ocean : BiomeType.Lake;
    }

    /// Thin beach strip around ocean/lake edges.
    private static void AddBeaches(HexTile[,] grid, int width, int height, float seaLevel)
    {
        for (int r = 0; r < height; r++)
        {
            for (int q = 0; q < width; q++)
            {
                var tile = grid[q, r];
                if (tile.Biome is BiomeType.Ocean or BiomeType.Lake or BiomeType.Beach) continue;
                if (tile.Elevation >= seaLevel + 0.07f) continue;

                foreach (var (dq, dr) in HexNeighborOffsets())
                {
                    int nq = q + dq, nr = r + dr;
                    if (nq < 0 || nq >= width || nr < 0 || nr >= height) continue;
                    if (grid[nq, nr].Biome is BiomeType.Ocean or BiomeType.Lake)
                    {
                        tile.Biome = BiomeType.Beach;
                        break;
                    }
                }
            }
        }
    }

    /// Shallow ocean tiles (elevation near sea level) that border land become Shallows.
    private static void AddShallows(HexTile[,] grid, int width, int height, float seaLevel)
    {
        var toConvert = new List<(int q, int r)>();
        for (int r = 0; r < height; r++)
        {
            for (int q = 0; q < width; q++)
            {
                var tile = grid[q, r];
                if (tile.Biome != BiomeType.Ocean) continue;
                if (tile.Elevation < seaLevel - 0.05f) continue;

                foreach (var (dq, dr) in HexNeighborOffsets())
                {
                    int nq = q + dq, nr = r + dr;
                    if (nq < 0 || nq >= width || nr < 0 || nr >= height) continue;
                    if (grid[nq, nr].Biome is not BiomeType.Ocean and not BiomeType.Lake)
                    {
                        toConvert.Add((q, r));
                        break;
                    }
                }
            }
        }
        foreach (var (q, r) in toConvert)
            grid[q, r].Biome = BiomeType.Shallows;
    }

    private void PlaceFeatures(HexTile[,] grid, int width, int height, int seed)
    {
        var rng = new Random(seed + 0x5EED);
        for (int r = 0; r < height; r++)
        {
            for (int q = 0; q < width; q++)
            {
                var tile = grid[q, r];
                if (tile.Biome is BiomeType.Ocean or BiomeType.Lake or BiomeType.River
                                or BiomeType.Shallows or BiomeType.Volcano) continue;

                foreach (var def in _featureProvider.All)
                {
                    if (!def.AllowedBiomes.Contains(tile.Biome)) continue;
                    if (rng.NextDouble() < def.Probability)
                    {
                        tile.FeatureId = def.Id;
                        break;
                    }
                }
            }
        }
    }

    /// Traces rivers downhill from mountain/snow sources to the nearest water body.
    private static void GenerateRivers(HexTile[,] grid, int width, int height, int seed, int count)
    {
        var rng     = new Random(seed + 1);
        var sources = new List<(int q, int r)>();

        for (int r = 0; r < height; r++)
            for (int q = 0; q < width; q++)
                if (grid[q, r].Biome is BiomeType.Mountain or BiomeType.Snow or BiomeType.Glacier)
                    sources.Add((q, r));

        // Fisher-Yates shuffle
        for (int i = sources.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (sources[i], sources[j]) = (sources[j], sources[i]);
        }

        int generated = 0;
        foreach (var (sq, sr) in sources)
        {
            if (generated >= count) break;
            if (!TraceRiver(grid, width, height, sq, sr, rng)) continue;
            generated++;
        }
    }

    private static bool TraceRiver(HexTile[,] grid, int width, int height, int startQ, int startR, Random rng)
    {
        int q = startQ, r = startR;
        bool reachedWater = false;

        for (int step = 0; step < 600; step++)
        {
            var tile = grid[q, r];
            if (tile.Biome is BiomeType.Ocean or BiomeType.Lake) { reachedWater = true; break; }
            if (tile.Biome is not BiomeType.Mountain and not BiomeType.Snow and not BiomeType.Glacier and not BiomeType.Beach)
                tile.Biome = BiomeType.River;

            // Collect all downhill neighbours; add a small random weight to avoid grid artifacts
            float myElev = tile.Elevation;
            (int nq, int nr, float score) best = (-1, -1, float.MaxValue);

            foreach (var (dq, dr) in HexNeighborOffsets())
            {
                int nq = q + dq, nr2 = r + dr;
                if (nq < 0 || nq >= width || nr2 < 0 || nr2 >= height) continue;
                float score = grid[nq, nr2].Elevation + (float)(rng.NextDouble() * 0.015);
                if (score < best.score)
                    best = (nq, nr2, score);
            }

            if (best.nq < 0 || best.score >= myElev + 0.015f) break; // no downhill route
            q = best.nq; r = best.nr;
        }
        return reachedWater;
    }

    private static List<HexTile> FlatList(HexTile[,] grid, int width, int height)
    {
        var list = new List<HexTile>(width * height);
        for (int r = 0; r < height; r++)
            for (int q = 0; q < width; q++)
                list.Add(grid[q, r]);
        return list;
    }

    public static (int dq, int dr)[] HexNeighborOffsets() =>
        [(-1, 0), (1, 0), (0, -1), (0, 1), (-1, 1), (1, -1)];

}
