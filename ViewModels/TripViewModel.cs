//using Java.Lang.Annotation;
using PackMeUp.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PackMeUp.ViewModels
{
    public class TripViewModel// : INotifyPropertyChanged
    {
        public Trip TripModel { get; }

        //public string Title => Model.Name;
        //public string Destination => Model.Destination;

        public string TripDatesRange => $"{TripModel.StartDate:dd MMM} - {TripModel.EndDate:dd MMM}";
        public string PackingSummary => "10 / 30 (30%)";
        //public string BackgroundImage => $"https://cdn.example.com/trips/{Model.Destination}.jpg";
        public string BackgroundImage => "italy.webp";

        public TripViewModel(Trip trip)
        {
            TripModel = trip;
        }

        // Jeśli chcesz wspierać bindingi dwukierunkowe:
        //public bool IsFavorite
        //{
        //    get => _isFavorite;
        //    set
        //    {
        //        if (_isFavorite != value)
        //        {
        //            _isFavorite = value;
        //            OnPropertyChanged(nameof(IsFavorite));
        //        }
        //    }
        //}
        //private bool _isFavorite;

        //public event PropertyChangedEventHandler? PropertyChanged;
        ////protected void OnPropertyChanged([CallerMemberName] string name = null)
        ////    => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //protected virtual void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
    }
}
