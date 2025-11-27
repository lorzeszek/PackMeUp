using CommunityToolkit.Mvvm.Input;
using PackMeUp.Extensions;
using PackMeUp.Models;
using PackMeUp.Services.Interfaces;
using PackMeUp.Views;
using System.Text.Json;
using System.Windows.Input;

namespace PackMeUp.ViewModels
{
    public partial class TripListViewModel : RealtimeViewModel<Trip, TripViewModel>
    {
        public ObservableRangeCollection<TripViewModel> Trips { get; } = new();

        protected override ObservableRangeCollection<TripViewModel> ItemsCollection => Trips;

        protected override TripViewModel MapToViewModel(Trip model) => new TripViewModel(model);

        protected override object GetId(Trip model) => model.Id;

        protected override object GetModelId(TripViewModel vm) => vm.TripModel.Id;

        protected override bool ShouldAdd(Trip model) => !model.IsInTrash;

        protected override bool ShouldRemove(Trip model) => model.IsInTrash;






        //private bool _isSubscribed = false;
        //public ObservableRangeCollection<Trip> Trips { get; } = new();
        //public ObservableRangeCollection<TripViewModel> Trips { get; } = new ObservableRangeCollection<TripViewModel>();

        public ICommand TripTappedCommand => new Command<TripViewModel>(OnTripTapped);
        public ICommand AddTripCommand => new Command(async () => await Task.Run(() => AddTrip("wycieczka 1 test")));
        public ICommand DeleteTripCommand => new Command<TripViewModel>(async (trip) => await Task.Run(() => DeleteTripAsync(trip)));
        public ICommand TrashTripCommand => new Command<TripViewModel>(async (trip) => await Task.Run(() => TrashTripAsync(trip)));

        public IRelayCommand LogoutCommand => new AsyncRelayCommand(Logout);


        public TripListViewModel(ISupabaseService supabase, ISessionService sessionService) : base(supabase, sessionService)
        {
            Title = "Moje wycieczki";

            //TripTappedCommand = new Command<Trip>(OnTripTapped);
        }

        //protected override async Task ExecuteRefreshCommand()
        //{
        //    //await LoadTripsAsync();
        //    //await InitializeRealtimeAsync();
        //}

        private async void OnTripTapped(TripViewModel trip)
        {
            if (trip == null)
                return;

            //await Shell.Current.GoToAsync($"{nameof(PackingListPage)}?tripId={trip.Id}");

            await Shell.Current.GoToAsync(nameof(PackingListPage), new Dictionary<string, object>
            {
                ["tripId"] = trip.TripModel.Id
            });
        }

        private async Task TrashTripAsync(TripViewModel trip)
        {
            try
            {
                var getTripResult = await _supabase.Client.From<Trip>().Where(x => x.Id == trip.TripModel.Id).Get();

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

        private async Task DeleteTripAsync(TripViewModel trip)
        {
            try
            {
                await _supabase.Client.From<Trip>().Delete(trip.TripModel);
            }
            catch (Supabase.Postgrest.Exceptions.PostgrestException ex)
            {
                // Logowanie pełnego wyjątku
                Console.WriteLine($"Error: {ex.Message}, {ex.StackTrace}");
            }
        }

        private async Task AddTrip(string destinationName)
        {
            try
            {
                await _supabase.Client.From<Trip>().Insert(new Trip { IsActive = true, Destination = destinationName, CreatedDate = DateTime.Now, User_id = Session.UserId });
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
            await InitializeRealtimeAsync(async () =>
            {
                var response = await _supabase.Client
                    .From<Trip>()
                    .Select("*")
                    .Where(x => x.IsActive == true && x.IsInTrash == false)
                    .Get();

                return response.Models ?? new List<Trip>();
            });

            var statsResponse = await _supabase.Client.Rpc("count_items_stats_for_all_trips", null);
            if (statsResponse?.Content != null)
            {
                var stats = JsonSerializer.Deserialize<List<TripItemsStats>>(statsResponse.Content);
                if (stats != null)
                {
                    foreach (var trip in Trips)
                    {
                        var stat = stats.FirstOrDefault(s => s.TripId == trip.TripModel.Id);
                        if (stat != null)
                        {
                            trip.PackingSummary = $"{stat.IsPackedCount} / {stat.IsNotPackedCount + stat.IsPackedCount}";
                        }
                    }
                }
            }

        }
        //public async Task InitializeRealtimeAsync()
        //{
        //    if (_supabase.Client == null)
        //        throw new Exception("Supabase Client is not initialized");

        //    try
        //    {
        //        IsBusy = true;
        //        IsRefreshing = true;

        //        if (!_isSubscribed)// || _subscription == null)
        //        {
        //            await RealtimeSubscriptionHelper.SubscribeTableChanges<Trip>(
        //                _supabase.Client,
        //                // INSERT handler
        //                newItem =>
        //                {
        //                    if (!newItem.IsInTrash)
        //                    {
        //                        //MainThread.BeginInvokeOnMainThread(() => Trips.Add(newItem));
        //                        MainThread.BeginInvokeOnMainThread(() => Trips.Add(new TripViewModel(newItem)));
        //                    }
        //                },
        //                // UPDATE handler
        //                updatedItem =>
        //                {
        //                    //var existing = Trips.FirstOrDefault(t => t.Id == updatedItem.Id);
        //                    var existing = Trips.FirstOrDefault(t => t.TripModel.Id == updatedItem.Id);
        //                    MainThread.BeginInvokeOnMainThread(() =>
        //                    {
        //                        if (updatedItem.IsInTrash)
        //                        {
        //                            if (existing != null) Trips.Remove(existing);
        //                        }
        //                        else
        //                        {
        //                            if (existing != null)
        //                            {
        //                                var i = Trips.IndexOf(existing);
        //                                //Trips[i] = updatedItem;
        //                                Trips[i] = new TripViewModel(updatedItem);
        //                            }
        //                            //else { Trips.Add(updatedItem); }
        //                            else { Trips.Add(new TripViewModel(updatedItem)); }
        //                        }
        //                    });
        //                },
        //                // DELETE handler
        //                deletedItem =>
        //                {
        //                    //var existing = Trips.FirstOrDefault(t => t.Id == deletedItem.Id);
        //                    var existing = Trips.FirstOrDefault(t => t.TripModel.Id == deletedItem.Id);
        //                    if (existing != null)
        //                    {
        //                        MainThread.BeginInvokeOnMainThread(() => Trips.Remove(existing));
        //                    }
        //                }
        //            );

        //            _isSubscribed = true;
        //        }

        //        // 🔹 Początkowe pobranie z filtrem
        //        //var response = await _supabase.Client
        //        //    .From<Trip>()
        //        //    .Select("*, Items:PackingItem(*)")
        //        //    .Where(x => x.IsInTrash == false)
        //        //    .Get();

        //        var response = await _supabase.Client
        //            .From<Trip>()
        //            .Select("*")
        //            .Where(x => x.IsActive == true)
        //            //.Where(x => x.IsInTrash == false)
        //            .Get();

        //        var tripsViewModels = response.Models.Select(x => new TripViewModel(x))?.ToList() ?? [];

        //        var statsRespons = await _supabase.Client.Rpc("count_items_stats_for_all_trips", null);

        //        string statsResponsJson = statsRespons?.Content ?? string.Empty;

        //        if (!string.IsNullOrEmpty(statsResponsJson))
        //        {
        //            var stats = JsonSerializer.Deserialize<List<TripItemsStats>>(statsResponsJson);

        //            if (stats != null)
        //            {
        //                foreach (var trip in tripsViewModels)
        //                {
        //                    var stat = stats.FirstOrDefault(s => s.TripId == trip.TripModel.Id);
        //                    if (stat != null)
        //                    {
        //                        var total = stat.IsPackedCount + stat.IsNotPackedCount;
        //                        var percent = total > 0 ? (double)stat.IsPackedCount / total * 100 : 0;
        //                        trip.PackingSummary = $"{stat.IsPackedCount} / {total} ({percent:F0}%)";
        //                    }
        //                }
        //            }

        //        }

        //        Trips.ReplaceRange(tripsViewModels);
        //    }
        //    finally
        //    {
        //        IsBusy = false;
        //        IsRefreshing = false;
        //    }
        //}

        private async Task Logout()
        {
            await _supabase.Client.Auth.SignOut();

            //_sessionService.ClearUser();

            await Shell.Current.GoToAsync("///StartPage");
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
