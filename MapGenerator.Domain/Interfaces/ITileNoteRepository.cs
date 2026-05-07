using MapGenerator.Domain.Models;

namespace MapGenerator.Domain.Interfaces;

public interface ITileNoteRepository
{
    Task AddNoteAsync(TileNote note);
    Task<List<TileNote>> GetNotesForTileAsync(int q, int r);
    Task DeleteAllAsync();
}
