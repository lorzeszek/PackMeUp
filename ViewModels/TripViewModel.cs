using PackMeUp.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PackMeUp.ViewModels
{
    public class TripViewModel : INotifyPropertyChanged
    {
        public Trip TripModel { get; }

        public string Destination => TripModel.Destination;

        public string TripDatesRange => GetTripDatesRange();

        private string _packingSummary = string.Empty;
        public string PackingSummary
        {
            get => _packingSummary;
            set
            {
                if (_packingSummary != value)
                {
                    _packingSummary = value;
                    OnPropertyChanged();
                }
            }
        }

        //public string BackgroundImage => $"https://cdn.example.com/trips/{Model.Destination}.jpg";
        public string BackgroundImage => "italy.webp";

        public TripViewModel(Trip trip)
        {
            TripModel = trip;
        }

        private string GetTripDatesRange()
        {
            var startDate = TripModel.StartDate;
            var endDate = TripModel.EndDate;

            if (startDate.HasValue && endDate.HasValue && startDate.Value.Year == endDate.Value.Year)
            {
                return $"{startDate:dd MMM} - {endDate:dd MMM} {startDate:yyyy}";
            }
            else if (startDate.HasValue && !endDate.HasValue)
            {
                return $"{startDate:dd MMM yyyy}";
            }
            else
                return $"{startDate:dd MMM yyyy} - {endDate:dd MMM yyyy}";
        }

        public void UpdateFromTrip(Trip trip)
        {
            TripModel.Destination = trip.Destination;
            TripModel.IsActive = trip.IsActive;
            TripModel.IsInTrash = trip.IsInTrash;
            TripModel.StartDate = trip.StartDate;
            TripModel.EndDate = trip.EndDate;
            TripModel.ModifiedDate = trip.ModifiedDate;
            TripModel.User_id = trip.User_id;

            OnPropertyChanged(nameof(Destination));
            OnPropertyChanged(nameof(TripModel.IsActive));
            OnPropertyChanged(nameof(TripModel.IsInTrash));
            OnPropertyChanged(nameof(TripModel.StartDate));
            OnPropertyChanged(nameof(TripModel.EndDate));
            OnPropertyChanged(nameof(TripModel.ModifiedDate));
            OnPropertyChanged(nameof(TripModel.User_id));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
