using PackMeUp.Extensions;
using PackMeUp.Helpers;
using PackMeUp.Models;
using PackMeUp.Services;
using PackMeUp.Views;
using System.Windows.Input;

namespace PackMeUp.ViewModels
{
    public partial class TripListViewModel : BaseViewModel
    {
        private bool _isSubscribed = false;
        public ObservableRangeCollection<Trip> Trips { get; } = new();

        public ICommand TripTappedCommand => new Command<Trip>(OnTripTapped);
        public ICommand AddTripCommand => new Command(async () => await Task.Run(() => AddTrip("wycieczka 1 test")));
        public ICommand DeleteTripCommand => new Command<Trip>(async (trip) => await Task.Run(() => DeleteTripAsync(trip)));
        public ICommand TrashTripCommand => new Command<Trip>(async (trip) => await Task.Run(() => TrashTripAsync(trip)));



        public TripListViewModel(ISupabaseService supabase) : base(supabase)
        {
            Title = "Moje wycieczki";

            //TripTappedCommand = new Command<Trip>(OnTripTapped);
        }

        //protected override async Task ExecuteRefreshCommand()
        //{
        //    //await LoadTripsAsync();
        //    //await InitializeRealtimeAsync();
        //}

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

        private async Task TrashTripAsync(Trip trip)
        {
            try
            {
                var getTripResult = await _supabase.Client.From<Trip>().Where(x => x.Id == trip.Id).Get();

                var selectedTrip = getTripResult.Models.First();

                selectedTrip.IsInTrash = true;

                var updateResponse = await _supabase.Client
                .From<Trip>()
                .Update(selectedTrip);

                //await InitializeRealtimeAsync();
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

            //await MainThread.InvokeOnMainThreadAsync(async () =>
            //{
            //    await LoadTripsAsync();
            //});
        }

        private async Task DeleteTripAsync(Trip trip)
        {
            try
            {
                await _supabase.Client.From<Trip>().Delete(trip);
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

            //await MainThread.InvokeOnMainThreadAsync(async () =>
            //{
            //    await LoadTripsAsync();
            //});
        }

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

            //await MainThread.InvokeOnMainThreadAsync(async () =>
            //{
            //    await LoadTripsAsync();
            //});
        }

        public async Task InitializeRealtimeAsync()
        {
            if (_supabase.Client == null)
                throw new Exception("Supabase Client is not initialized");

            try
            {
                IsBusy = true;
                IsRefreshing = true;

                if (!_isSubscribed)// || _subscription == null)
                {
                    await RealtimeSubscriptionHelper.SubscribeTableChanges<Trip>(
                        _supabase.Client,
                        // INSERT handler
                        newItem =>
                        {
                            if (!newItem.IsInTrash)
                            {
                                MainThread.BeginInvokeOnMainThread(() => Trips.Add(newItem));
                            }
                        },
                        // UPDATE handler
                        updatedItem =>
                        {
                            var existing = Trips.FirstOrDefault(t => t.Id == updatedItem.Id);
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                if (updatedItem.IsInTrash)
                                {
                                    if (existing != null) Trips.Remove(existing);
                                }
                                else
                                {
                                    if (existing != null)
                                    {
                                        var i = Trips.IndexOf(existing);
                                        Trips[i] = updatedItem;
                                    }
                                    else { Trips.Add(updatedItem); }
                                }
                            });
                        },
                        // DELETE handler
                        deletedItem =>
                        {
                            var existing = Trips.FirstOrDefault(t => t.Id == deletedItem.Id);
                            if (existing != null)
                            {
                                MainThread.BeginInvokeOnMainThread(() => Trips.Remove(existing));
                            }
                        }
                    );

                    _isSubscribed = true;
                }

                // 🔹 Początkowe pobranie z filtrem
                var response = await _supabase.Client
                    .From<Trip>()
                    .Select("*, Items:PackingItem(*)")
                    .Where(x => x.IsInTrash == false)
                    .Get();

                Trips.ReplaceRange(response.Models);
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        public Task DisposeRealtimeAsync()
        {
            if (_subscription != null)
            {
                _subscription.Unsubscribe();
            }

            return Task.CompletedTask;
        }

        protected override async Task OnNavigatedToAsync(IDictionary<string, object> query)
        {
            await InitializeRealtimeAsync();
        }
    }
}
