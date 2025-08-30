using PackMeUp.Extensions;
using PackMeUp.Models;
using PackMeUp.Services;
using PackMeUp.Views;
using System.Windows.Input;

namespace PackMeUp.ViewModels
{
    public partial class TripListViewModel : BaseViewModel
    {
        public ObservableRangeCollection<Trip> Trips { get; } = new();

        public ICommand AddTripCommand { get; }
        public ICommand TripTappedCommand { get; }



        public TripListViewModel(ISupabaseService supabase) : base(supabase)
        {
            Title = "Moje wycieczki";

            AddTripCommand = new Command(async () => await Task.Run(() => AddTrip("wycieczka 1 test")));


            //AddTripCommand = new Command(AddTrip);
            //RefreshCommand = new Command(async () => await LoadTripsAsync());
            TripTappedCommand = new Command<Trip>(OnTripTapped);

            _ = LoadTripsAsync();
        }

        protected override async Task ExecuteRefreshCommand()
        {
            await LoadTripsAsync();
        }

        private async void OnTripTapped(Trip trip)
        {
            if (trip == null)
                return;

            //await Shell.Current.GoToAsync($"{nameof(PackingListPage)}?tripId={trip.Id}");

            await Shell.Current.GoToAsync(nameof(PackingListPage), new Dictionary<string, object>
            {
                ["tripId"] = trip.Id
            });
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
                IsRefreshing = true;

                var response = await _supabase.Client.From<Trip>().Select("*, Items:PackingItem(*)").Get();

                var loadedTrips = response.Models
                    .Select(x => new Trip
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Items = x.Items,
                        CreatedDate = x.CreatedDate,
                    })
                    .ToList();

                Trips.ReplaceRange(loadedTrips);
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }
    }
}
