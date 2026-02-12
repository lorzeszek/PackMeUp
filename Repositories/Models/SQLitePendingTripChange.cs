using SQLite;

namespace PackMeUp.Repositories.Models
{
    [Table("SQLitePendingTripChange")]
    public class SQLitePendingTripChange
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        //public int Id { get; set; }

        //public int TripId { get; set; }
        public string LocalUserId { get; set; } = string.Empty;

        public string Operation { get; set; } = string.Empty; // "Add", "Update", "Delete"
        public string TripJson { get; set; } = string.Empty;  // serializacja Trip do JSON
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
