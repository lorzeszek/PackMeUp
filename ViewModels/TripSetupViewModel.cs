using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PackMeUp.Models.DTO;
using PackMeUp.Repositories.Interfaces;
using PackMeUp.Services.Interfaces;

namespace PackMeUp.ViewModels
{
    public partial class TripSetupViewModel : BaseViewModel
    {
        //private readonly ITripRepository _tripRepository;
        private readonly ILocalUserService _localUserService;

        //public AsyncRelayCommand CreateTripCommand => new AsyncRelayCommand(CreateTripAsync, CanCreateTrip);

        //public IAsyncRelayCommand CreateTripCommand { get; }


        public TripSetupViewModel(ILocalUserService localUserService, ISupabaseService supabase, ISessionService sessionService, IPackingItemRepository packingItemRepository, ITripRepository tripRepository) : base(supabase, sessionService, packingItemRepository, tripRepository)
        {
            //_tripRepository = tripRepository;
            _localUserService = localUserService;

            //CreateTripCommand = new AsyncCommand(CreateTripAsync, CanCreateTrip);
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

        // ====== COMMAND ======

        [RelayCommand(CanExecute = nameof(CanCreateTrip))]
        private async Task CreateTripAsync()
        {
            var localUser = await _localUserService.CreateLocalUserAsync();

            Session.SetLocalUser(localUser.LocalUserId);

            var trip = new TripDTO
            {
                LocalUserId = localUser.LocalUserId,
                RemoteUserId = Session.UserId,
                CreatedDate = DateTime.Now,
                StartDate = StartDate,
                EndDate = EndDate,
                Destination = Destination,
                IsActive = true
            };

            await _tripRepository.AddTripAsync(trip);
            await Shell.Current.GoToAsync("//TripList");
        }

        private bool CanCreateTrip()
            => !string.IsNullOrWhiteSpace(Destination)
               && EndDate >= StartDate;
    }
}
