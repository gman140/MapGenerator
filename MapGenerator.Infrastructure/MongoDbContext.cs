using MapGenerator.Domain.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace MapGenerator.Infrastructure;

public class MongoDbContext
{
    static MongoDbContext()
    {
        RegisterMap<HexTile>();
        RegisterMap<Player>();
        RegisterMap<ChatMessage>();
        RegisterMap<PlayerTileVisit>();
        RegisterMap<MapConfig>();
        RegisterMap<MapGenerationOptions>();
        RegisterMap<TileNote>();
        RegisterMap<TileInventory>();
        RegisterMap<Settlement>();
        RegisterMap<SettlementTile>();
        RegisterMap<Road>();
        RegisterMap<RoadPoint>();
    }

    private static void RegisterMap<T>() where T : class
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
        {
            BsonClassMap.RegisterClassMap<T>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                var idMember = cm.GetMemberMap("Id");
                if (idMember != null)
                {
                    idMember.SetIdGenerator(StringObjectIdGenerator.Instance)
                            .SetSerializer(new StringSerializer(BsonType.ObjectId));
                }
            });
        }
    }

    private readonly IMongoDatabase _db;

    public MongoDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _db = client.GetDatabase(databaseName);
    }

    public IMongoCollection<HexTile> Tiles => _db.GetCollection<HexTile>("tiles");
    public IMongoCollection<Player> Players => _db.GetCollection<Player>("players");
    public IMongoCollection<ChatMessage> ChatMessages => _db.GetCollection<ChatMessage>("chatMessages");
    public IMongoCollection<PlayerTileVisit> Visits => _db.GetCollection<PlayerTileVisit>("visits");
    public IMongoCollection<MapConfig> MapConfigs => _db.GetCollection<MapConfig>("mapConfigs");
    public IMongoCollection<TileNote> TileNotes => _db.GetCollection<TileNote>("tileNotes");
    public IMongoCollection<TileInventory> TileInventories => _db.GetCollection<TileInventory>("tileInventories");
    public IMongoCollection<Settlement> Settlements => _db.GetCollection<Settlement>("settlements");
    public IMongoCollection<Road> Roads => _db.GetCollection<Road>("roads");
}
