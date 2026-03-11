namespace PackMeUp.Models.DTO
{
    public class TripDTO
    {
        public int RemoteTripId { get; set; }
        public int LocalTripId { get; set; }
        public string LocalUserId { get; set; }
        public string RemoteUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Destination { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsInTrash { get; set; }
    }
}
