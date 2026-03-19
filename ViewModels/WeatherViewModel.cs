using PackMeUp.Interfaces;
using PackMeUp.Repositories.Interfaces;
using PackMeUp.Services.Interfaces;

namespace PackMeUp.ViewModels
{
    public class WeatherViewModel : BaseViewModel
    {
        public WeatherViewModel(ILocalUserService localUserService, ISupabaseService supabase, ISessionService sessionService, IPackingItemRepository packingItemRepository, ITripRepository tripRepository, IGoogleAuthService googleAuthService) : base(localUserService, supabase, sessionService, packingItemRepository, tripRepository, googleAuthService)
        {
        }
    }
}
