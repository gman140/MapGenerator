using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public class ChatService
{
    private readonly IChatRepository _chatRepo;
    private readonly IPlayerTileVisitRepository _visitRepo;

    public ChatService(IChatRepository chatRepo, IPlayerTileVisitRepository visitRepo)
    {
        _chatRepo = chatRepo;
        _visitRepo = visitRepo;
    }

    public async Task<ChatMessage> SendLocalAsync(Player player, string content)
    {
        var msg = new ChatMessage
        {
            Content = content.Trim()[..Math.Min(content.Trim().Length, 500)],
            SenderName = player.Username,
            SenderId = player.Id,
            TileQ = player.Q,
            TileR = player.R,
            IsWorldChat = false,
            SentAt = DateTime.UtcNow
        };
        return await _chatRepo.SaveMessageAsync(msg);
    }

    public async Task<ChatMessage> SendWorldAsync(Player player, string content)
    {
        var msg = new ChatMessage
        {
            Content = content.Trim()[..Math.Min(content.Trim().Length, 500)],
            SenderName = player.Username,
            SenderId = player.Id,
            IsWorldChat = true,
            SentAt = DateTime.UtcNow
        };
        return await _chatRepo.SaveMessageAsync(msg);
    }

    public async Task<List<ChatMessage>> GetTileHistoryAsync(string playerId, int q, int r)
    {
        var visits = await _visitRepo.GetVisitsForPlayerOnTileAsync(playerId, q, r);
        return await _chatRepo.GetTileMessagesForVisitsAsync(visits);
    }

    public Task<List<ChatMessage>> GetWorldHistoryAsync() => _chatRepo.GetWorldMessagesAsync();
}
