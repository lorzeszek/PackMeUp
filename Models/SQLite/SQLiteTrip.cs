using PackMeUp.Models.Enums;
using SQLite;

namespace PackMeUp.Models.SQLite
{
    [Table("SQLiteTrip")]
    public class SQLiteTrip
    {
        [PrimaryKey, AutoIncrement]
        public int LocalTripId { get; set; }
        public int? RemoteTripId { get; set; }
        public string LocalUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Destination { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsInTrash { get; set; }
        public CoverThemeType CoverTheme { get; set; }
        public string CoverImagePath { get; set; }
    }
}
