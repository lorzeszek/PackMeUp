using Packo.Models.DTO;

namespace Packo.Services.Interfaces
{
    public interface ICoverCacheService
    {
        string GetOrCreateCover(TripDTO trip, string packingSummary);
    }
}
