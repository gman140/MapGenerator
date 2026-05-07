using MapGenerator.Domain.Models;

namespace MapGenerator.Domain.Interfaces;

public interface IChatRepository
{
    Task<ChatMessage> SaveMessageAsync(ChatMessage message);
    Task<List<ChatMessage>> GetTileMessagesForVisitsAsync(List<PlayerTileVisit> visits);
    Task<List<ChatMessage>> GetWorldMessagesAsync(int limit = 100);
    Task RetainMessagesFromDeletedPlayerAsync(string playerId, string displayName);
    Task DeleteAllAsync();
}
