using PackMeUp.Models;
using PackMeUp.Models.DTO;

namespace PackMeUp.Helpers
{
    public static class Mappers
    {
        public static TripDTO MapToTripDTO(Trip trip)
        {
            return new TripDTO()
            {
                RemoteId = trip.Id,
                RemoteUserId = trip.User_id,
                LocalUserId = trip.ClientId,
                IsActive = trip.IsActive,
                Destination = trip.Destination,
                CreatedDate = trip.CreatedDate,
                ModifiedDate = null,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                IsInTrash = trip.IsInTrash,
            };
        }

        public static Trip MapToTrip(TripDTO trip)
        {
            return new Trip()
            {
                Id = trip.RemoteId,
                User_id = trip.RemoteUserId,
                ClientId = trip.LocalUserId,
                IsActive = trip.IsActive,
                Destination = trip.Destination,
                CreatedDate = trip.CreatedDate,
                ModifiedDate = null,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                IsInTrash = trip.IsInTrash,
            };
        }
    }
}
