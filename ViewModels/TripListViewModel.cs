using PackMeUp.Models;
using PackMeUp.Services;
using PackMeUp.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PackMeUp.ViewModels
{
    public class TripListViewModel : BaseViewModel
    {
        private ObservableCollection<Trip> _trips = new();
        public ObservableCollection<Trip> Trips { get => _trips; set { SetProperty(ref _trips, value); } }

        public ICommand AddTripCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand TripTappedCommand { get; }



        public TripListViewModel(ISupabaseService supabase) : base(supabase)
        {
            Title = "Moje wycieczki";

            AddTripCommand = new Command(async () => await Task.Run(() => AddTrip("wycieczka 1 test")));


            //AddTripCommand = new Command(AddTrip);
            RefreshCommand = new Command(async () => await LoadTripsAsync());
            TripTappedCommand = new Command<Trip>(OnTripTapped);

            _ = LoadTripsAsync();
        }

        private async void OnTripTapped(Trip trip)
        {
            if (trip == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(PackingListPage)}?tripId={trip.Id}");
            //await Shell.Current.GoToAsync($"packinglist?tripId={trip.Id}");
        }

        //private void AddTrip()
        //{
        //    Trips.Add(new Trip { Name = $"Nowa wycieczka {Trips.Count + 1}" });
        //}

        private async Task AddTrip(string newListName)
        {
            try
            {
                await _supabase.Client.From<Trip>().Insert(new Trip { Name = newListName, CreatedDate = DateTime.Now });
            }
            catch (Supabase.Postgrest.Exceptions.PostgrestException ex)
            {
                // Logowanie pełnego wyjątku
                Console.WriteLine($"Error: {ex.Message}, {ex.StackTrace}");
            }
            //catch (Exception ex) 
            //{ 
            //    Console.WriteLine(ex.Message);
            //}

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await LoadTripsAsync();
            });
        }

        public async Task LoadTripsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var response = await _supabase.Client.From<Trip>().Select("*, Items:PackingItem(*)").Get();


                if (response.Models.Count != 0)
                {
                    Trips.Clear();

                    Trips = new ObservableCollection<Trip>();

                    response.Models.ForEach(x => Trips.Add(new Trip
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Items = x.Items,
                        CreatedDate = x.CreatedDate,
                    }));
                }
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }
    }
}
