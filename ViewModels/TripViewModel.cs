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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
