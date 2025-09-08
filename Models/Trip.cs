using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.ComponentModel;

namespace PackMeUp.Models
{
    [Table("Trip")]
    public partial class Trip : BaseModel, INotifyPropertyChanged
    {
        [PrimaryKey("Id", false)]
        public int Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [Column("ModifiedDate")]
        public DateTime? ModifiedDate { get; set; }

        private bool _isInTrash;
        [Column("IsInTrash")]
        public bool IsInTrash
        {
            get => _isInTrash;
            set
            {
                if (_isInTrash != value)
                {
                    _isInTrash = value;
                    OnPropertyChanged(nameof(IsInTrash));
                }
            }
        }

        [Reference(typeof(PackingItem), ReferenceAttribute.JoinType.Left)]
        public List<PackingItem> Items { get; set; } = new();


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
