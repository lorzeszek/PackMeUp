using PackMeUp.Models.DTO;

namespace PackMeUp.Services.Interfaces
{
    public interface ICoverCacheService
    {
        string GetOrCreateCover(TripDTO trip, string packingSummary);
    }
}
