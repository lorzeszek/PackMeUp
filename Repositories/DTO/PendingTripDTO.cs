namespace PackMeUp.Repositories.DTO
{
    public class PendingTripDTO
    {
        //public int? Id { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string User_id { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsInTrash { get; set; }
    }
}
