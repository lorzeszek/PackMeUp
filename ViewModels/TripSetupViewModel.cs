using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PackMeUp.Interfaces;
using PackMeUp.Models.DTO;
using PackMeUp.Popups;
using PackMeUp.Repositories.Interfaces;
using PackMeUp.Services.Interfaces;

namespace PackMeUp.ViewModels
{
    public partial class TripSetupViewModel : BaseViewModel
    {
        private readonly IPackingSuggestionService _packingSuggestionService;

        public TripSetupViewModel(ILocalUserService localUserService, ISupabaseService supabase, ISessionService sessionService, IPackingItemRepository packingItemRepository, ITripRepository tripRepository, IGoogleAuthService googleAuthService, IPackingSuggestionService packingSuggestionService) : base(localUserService, supabase, sessionService, packingItemRepository, tripRepository, googleAuthService)
        {
            _packingSuggestionService = packingSuggestionService;
        }

        // ====== INPUT ======
        [ObservableProperty]
        [NotifyCanExecuteChangedFor("CreateTripCommand")]
        private string destination = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor("CreateTripCommand")]
        private DateTime startDate = DateTime.Today;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor("CreateTripCommand")]
        private DateTime endDate = DateTime.Today.AddDays(1);

        partial void OnStartDateChanged(DateTime value)
        {
            if (value < DateTime.Today)
                StartDate = DateTime.Today;

            if (EndDate < value)
                EndDate = value;
        }

        partial void OnEndDateChanged(DateTime value)
        {
            if (value < StartDate)
                EndDate = StartDate;
        }

        // ====== COMMAND ======

        [RelayCommand(CanExecute = nameof(CanCreateTrip))]
        private async Task CreateTripAsync()
        {
            if (Session.LocalUserId == null)
            {
                var localUser = await _localUserService.CreateLocalUserAsync();

                Session.SetLocalUser(localUser.LocalUserId);
            }

            var trip = new TripDTO
            {
                LocalUserId = Session.LocalUserId,
                RemoteUserId = Session.UserId,
                CreatedDate = DateTime.Now,
                StartDate = StartDate,
                EndDate = EndDate,
                Destination = Destination,
                IsActive = true
            };

            await _tripRepository.AddTripAsync(trip);

            var proposeListPopup = new ConfirmPopup("Lista pakowania", "Czy chcesz wygenerować listę rzeczy do spakowania?");

            var result = await Application.Current.MainPage.ShowPopupAsync<bool>(proposeListPopup);

            var proposeListPopupResult = result.Result;

            if (proposeListPopupResult)
            {
                try
                {
                    var proposeListItems = await _packingSuggestionService.GenerateItemsAsync(Destination, StartDate, EndDate);

                    var addSelectedItemsPopup = new PackingItemsPopup(proposeListItems);

                    _ = Application.Current.MainPage.ShowPopupAsync(addSelectedItemsPopup);

                    var selectedItemsNames = await addSelectedItemsPopup.ResultSource.Task;

                    if (selectedItemsNames?.Count > 0)
                    {
                        var createdTrip = await _tripRepository.GetTripAsync(trip);

                        var newPackingItems = selectedItemsNames.Select(itemName => new PackingItemDTO { Name = itemName, Category = 3, RemoteTripId = createdTrip.RemoteTripId, LocalTripId = createdTrip.LocalTripId, RemoteUserId = Session.UserId, LocalUserId = Session.LocalUserId, CreatedDate = DateTime.Now }).ToList();

                        await _packingItemRepository.AddPackingItemsAsync(newPackingItems);
                    }
                }
                catch (Exception ex)
                {
                    // Handle error
                }
            }



            // Pop TripSetupPage off the Home tab's stack before switching tabs
            await Shell.Current.Navigation.PopAsync(false);
            await Shell.Current.GoToAsync("//TripList");
        }

        private bool CanCreateTrip()
            => !string.IsNullOrWhiteSpace(Destination)
               && EndDate >= StartDate;
    }
}
