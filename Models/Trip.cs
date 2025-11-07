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

        [Column("Destination")]
        public string Destination { get; set; }

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [Column("ModifiedDate")]
        public DateTime? ModifiedDate { get; set; }

        [Column("StartDate")]
        public DateTime? StartDate { get; set; }

        [Column("EndDate")]
        public DateTime? EndDate { get; set; }

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

        private bool _isActive;
        [Column("IsActive")]
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged(nameof(IsActive));
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
