using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;
using MongoDB.Driver;

namespace MapGenerator.Infrastructure.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly MongoDbContext _ctx;

    public ChatRepository(MongoDbContext ctx)
    {
        _ctx = ctx;
        _ctx.ChatMessages.Indexes.CreateOne(new CreateIndexModel<ChatMessage>(
            Builders<ChatMessage>.IndexKeys.Combine(
                Builders<ChatMessage>.IndexKeys.Ascending(m => m.TileQ),
                Builders<ChatMessage>.IndexKeys.Ascending(m => m.TileR),
                Builders<ChatMessage>.IndexKeys.Ascending(m => m.SentAt))));
        _ctx.ChatMessages.Indexes.CreateOne(new CreateIndexModel<ChatMessage>(
            Builders<ChatMessage>.IndexKeys.Combine(
                Builders<ChatMessage>.IndexKeys.Ascending(m => m.IsWorldChat),
                Builders<ChatMessage>.IndexKeys.Descending(m => m.SentAt))));
    }

    public async Task<ChatMessage> SaveMessageAsync(ChatMessage message)
    {
        await _ctx.ChatMessages.InsertOneAsync(message);
        return message;
    }

    public async Task<List<ChatMessage>> GetTileMessagesForVisitsAsync(List<PlayerTileVisit> visits)
    {
        if (visits.Count == 0) return [];

        var filters = visits.Select(v =>
        {
            var end = v.LeftAt ?? DateTime.UtcNow;
            return Builders<ChatMessage>.Filter.And(
                Builders<ChatMessage>.Filter.Eq(m => m.TileQ, v.Q),
                Builders<ChatMessage>.Filter.Eq(m => m.TileR, v.R),
                Builders<ChatMessage>.Filter.Eq(m => m.IsWorldChat, false),
                Builders<ChatMessage>.Filter.Gte(m => m.SentAt, v.ArrivedAt),
                Builders<ChatMessage>.Filter.Lte(m => m.SentAt, end));
        }).ToList();

        var combined = Builders<ChatMessage>.Filter.Or(filters);
        return await _ctx.ChatMessages
            .Find(combined)
            .SortBy(m => m.SentAt)
            .ToListAsync();
    }

    public Task<List<ChatMessage>> GetWorldMessagesAsync(int limit = 100) =>
        _ctx.ChatMessages
            .Find(m => m.IsWorldChat)
            .SortByDescending(m => m.SentAt)
            .Limit(limit)
            .ToListAsync()
            .ContinueWith(t => { t.Result.Reverse(); return t.Result; });

    public Task RetainMessagesFromDeletedPlayerAsync(string playerId, string displayName) =>
        // Messages already stored with SenderName; just nullify SenderId so the account is truly gone
        _ctx.ChatMessages.UpdateManyAsync(
            m => m.SenderId == playerId,
            Builders<ChatMessage>.Update.Set(m => m.SenderId, null));
}
