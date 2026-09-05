using System.ComponentModel;

namespace Packo.Models
{
    public class TripDocument : INotifyPropertyChanged
    {
        public Guid Id { get; set; }
        public Guid TripId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string LocalPath { get; set; } = string.Empty;
        public DateTime AddedAt { get; set; }
        public long FileSize { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
