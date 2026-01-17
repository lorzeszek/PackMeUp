using SQLite;

namespace PackMeUp.Models.SQLite
{
    [Table("SQLitePackingItem")]
    public class SQLitePackingItem
    {
        [PrimaryKey]
        public int SupabaseItemId { get; set; }
        public int TripId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string User_id { get; set; } = string.Empty;
        public bool IsPacked { get; set; }
        public int Category { get; set; }
    }
}
