using MapGenerator.Domain.Models;

namespace MapGenerator.Domain.Interfaces;

public interface IFeedbackRepository
{
    Task SaveAsync(Feedback feedback);
    Task<List<Feedback>> GetAllAsync();
}
