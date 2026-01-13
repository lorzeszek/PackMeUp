using CommunityToolkit.Mvvm.Input;
using PackMeUp.Extensions;
using PackMeUp.Models;
using PackMeUp.Popups;
using PackMeUp.Repositories.Interfaces;
using PackMeUp.Services.Interfaces;
using PackMeUp.Views;
using System.Windows.Input;
using UXDivers.Popups.Services;

namespace PackMeUp.ViewModels
{
    public partial class TripListViewModel : BaseViewModel
    {
        public ObservableRangeCollection<TripViewModel> Trips { get; } = new();

        public ICommand TripTappedCommand => new Command<TripViewModel>(OnTripTapped);
        public ICommand AddTripCommand => new Command(async () => await AddTrip("wycieczka 1 test"));
        public ICommand DeleteTripCommand => new Command<TripViewModel>(async (trip) => await DeleteTripAsync(trip));
        public ICommand TrashTripCommand => new Command<TripViewModel>(TrashTripAsync);
        public IRelayCommand LogoutCommand => new AsyncRelayCommand(async () => Logout());


        public TripListViewModel(ISupabaseService supabase, ISessionService sessionService, IPackingItemRepository packingItemRepository, ITripRepository tripRepository) : base(supabase, sessionService, packingItemRepository, tripRepository)
        {
            Title = "Moje wycieczki";
        }

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

        private async void TrashTripAsync(TripViewModel trip)
        {
            //trip.TripModel.IsInTrash = true;

            //await _tripRepository.UpdateTripAsync(trip.TripModel);
            ////await _tripRepository.DeleteTripAsync(trip.TripModel);
            ///

            await _tripRepository.DeleteTripAsync(trip.TripModel);


            ////await LoadData();


            //var updated = trip.TripModel;

            //if (updated != null)
            //{
            //    updated.IsInTrash = true;
            //    await _tripRepository.UpdateTripAsync(updated);
            //}


            //await _tripRepository.UpdateTripAsync(trip.TripModel);
        }

        private async Task DeleteTripAsync(TripViewModel trip)
        {
            try
            {
                await _tripRepository.DeleteTripAsync(trip.TripModel);
            }
            catch (Supabase.Postgrest.Exceptions.PostgrestException ex)
            {
                // Logowanie pełnego wyjątku
                Console.WriteLine($"Error: {ex.Message}, {ex.StackTrace}");
            }
        }

        private async Task AddTrip(string destinationName)
        {
            await _tripRepository.AddTripAsync(new Trip { IsActive = true, Destination = destinationName, CreatedDate = DateTime.Now, User_id = Session.UserId, ClientId = Guid.NewGuid().ToString() });
        }

        public async void Logout()
        {
            var popup = new ConfirmationPopup();
            var parameters = new Dictionary<string, object?>
            {
                { "message", "Do you want to delete this item?" }
            };

            bool confirmed = await IPopupService.Current.PushAsync(popup, parameters);

            if (confirmed)
            {
                //_tripRepository.TripChanged -= OnTripChanged;
                await _tripRepository.UnsubscribeFromTripChangesAsync();
                await _packingItemRepository.UnsubscribeFromPackingItemChangesAsync();
                await _supabase.Client.Auth.SignOut();
                await Shell.Current.GoToAsync("///StartPage");
            }



            //var popup = new SimpleActionPopup()
            //{
            //    Title = "Log out",
            //    Text = "Do you want to continue?",
            //    ActionButtonText = "Yes",
            //    SecondaryActionButtonText = "Cancel",
            //    ActionButtonCommand = new Command(async () =>
            //    {
            //        await IPopupService.Current.PopAsync();
            //        await _supabase.Client.Auth.SignOut();
            //        await Shell.Current.GoToAsync("///StartPage");
            //    })
            //};

        }

        public Task DisposeRealtimeAsync()
        {
            if (_subscription != null)
            {
                _subscription.Unsubscribe();
            }

            return Task.CompletedTask;
        }



        private void OnTripChanged(Trip trip, string operation)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                switch (operation)
                {
                    case "INSERT":
                        Trips.Add(new TripViewModel(trip));
                        break;
                    case "UPDATE":
                        var existing = Trips.FirstOrDefault(t => t.TripModel.Id == trip.Id);
                        if (existing != null)
                        {
                            Trips.Remove(existing);
                            existing.UpdateFromTrip(trip);
                        }
                        //
                        break;
                    case "DELETE":
                        var toRemove = Trips.FirstOrDefault(t => t.TripModel.Id == trip.Id);
                        if (toRemove != null)
                            Trips.Remove(toRemove);
                        break;
                }
            });
        }

        private async Task LoadData()
        {
            var tripsWithStats = await _tripRepository.GetActiveTripsWithStatsAsync();

            Trips.ReplaceRange(tripsWithStats.Select(x => new TripViewModel(x.Trip)));

            //Trips.Clear();

            //foreach (var item in tripsWithStats)
            //{
            //    Trips.Add(new TripViewModel(item.Trip)
            //    {
            //        PackingSummary = item.PackingSummary
            //    });
            //}
        }

        protected override async Task OnNavigatedToAsync(IDictionary<string, object> query)
        {
            _tripRepository.TripChanged -= OnTripChanged;
            _tripRepository.TripChanged += OnTripChanged;

            await LoadData();
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> query)
        {
            _tripRepository.TripChanged -= OnTripChanged;
            await _tripRepository.SyncPendingChangesAsync();
        }
    }
}
