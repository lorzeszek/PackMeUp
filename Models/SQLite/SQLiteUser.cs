using SQLite;

namespace PackMeUp.Models.SQLite
{
    [Table("SQLiteUser")]
    public class SQLiteUser
    {
        [PrimaryKey]
        public string LocalUserId { get; set; } // GUID
        public string? SupabaseUserId { get; set; } // null
        public DateTime CreatedDate { get; set; }
    }
}
