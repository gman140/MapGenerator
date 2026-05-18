using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;
using MongoDB.Driver;

namespace MapGenerator.Infrastructure.Repositories;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly IMongoCollection<Feedback> _col;

    public FeedbackRepository(MongoDbContext ctx) =>
        _col = ctx.Feedback;

    public Task SaveAsync(Feedback feedback) =>
        _col.InsertOneAsync(feedback);

    public async Task<List<Feedback>> GetAllAsync() =>
        await _col.Find(_ => true)
                  .SortByDescending(f => f.SubmittedAt)
                  .ToListAsync();
}
