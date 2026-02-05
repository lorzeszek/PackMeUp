using SQLite;

namespace PackMeUp.Models.SQLite
{
    [Table("SQLiteTrip")]
    public class SQLiteTrip
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string ClientId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Destination { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsInTrash { get; set; }
    }
}
