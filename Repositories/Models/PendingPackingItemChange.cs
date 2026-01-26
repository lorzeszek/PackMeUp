using SQLite;

namespace PackMeUp.Repositories.Models
{
    public class PendingPackingItemChange
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        //public int TripId { get; set; }
        public string Operation { get; set; } // "Add", "Update", "Delete"
        public string PackingItemJson { get; set; }  // serializacja Trip do JSON
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
