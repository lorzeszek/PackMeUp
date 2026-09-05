using SQLite;

namespace Packo.Models.SQLite
{
    [Table("TripDocument")]
    public class SQLiteTripDocument
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public Guid TripId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string LocalPath { get; set; } = string.Empty;
        public DateTime AddedAt { get; set; }
        public long FileSize { get; set; }
    }
}
