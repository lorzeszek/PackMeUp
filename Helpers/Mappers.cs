using PackMeUp.Models;
using PackMeUp.Models.DTO;
using PackMeUp.Models.Supabase;

namespace PackMeUp.Helpers
{
    public static class Mappers
    {
        public static TripDTO MapToTripDTO(TripSupabase trip)
        {
            return new TripDTO()
            {
                RemoteTripId = trip.Id,
                LocalTripId = trip.LocalTripId,
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

        public static TripSupabase MapToTripSupabase(TripDTO trip)
        {
            return new TripSupabase()
            {
                Id = trip.RemoteTripId,
                LocalTripId = trip.LocalTripId,
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

        public static PackingItemSupabase MapToPackingItemSupabase(PackingItemDTO item)
        {
            return new PackingItemSupabase()
            {
                Id = item.RemotePackingItemId ?? 0,
                TripId = item.RemoteTripId.Value,
                Name = item.Name,
                CreatedDate = item.CreatedDate,
                ModifiedDate = item.ModifiedDate,
                User_id = item.RemoteUserId,
                IsPacked = item.IsPacked,
                Category = item.Category
            };
        }

        public static PackingItemDTO MapToPackingItemDTO(PackingItemSupabase item)
        {
            return new PackingItemDTO()
            {
                RemotePackingItemId = item.Id,
                RemoteTripId = item.TripId,
                Name = item.Name,
                CreatedDate = item.CreatedDate,
                ModifiedDate = item.ModifiedDate,
                RemoteUserId = item.User_id,
                IsPacked = item.IsPacked,
                Category = item.Category
            };
        }

        public static PackingItemDTO MapToPackingItemDTO(PackingItem item)
        {
            return new PackingItemDTO()
            {
                LocalPackingItemId = item.LocalId,
                RemotePackingItemId = item.RemoteId,
                RemoteTripId = item.RemoteTripId,
                LocalTripId = item.LocalTripId,
                Name = item.Name,
                CreatedDate = item.CreatedDate,
                ModifiedDate = item.ModifiedDate,
                RemoteUserId = item.RemoteUserId,
                LocalUserId = item.LocalUserId,
                IsPacked = item.IsPacked,
                Category = item.Category
            };
        }

        public static PackingItem MapToPackingItem(PackingItemDTO item)
        {
            return new PackingItem()
            {
                LocalId = item.LocalPackingItemId,
                RemoteId = item.RemotePackingItemId,
                RemoteTripId = item.RemoteTripId,
                LocalTripId = item.LocalTripId,
                Name = item.Name,
                CreatedDate = item.CreatedDate,
                ModifiedDate = item.ModifiedDate,
                RemoteUserId = item.RemoteUserId,
                LocalUserId = item.LocalUserId,
                IsPacked = item.IsPacked,
                Category = item.Category
            };
        }
    }
}
