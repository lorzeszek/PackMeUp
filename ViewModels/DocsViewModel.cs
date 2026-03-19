using PackMeUp.Interfaces;
using PackMeUp.Repositories.Interfaces;
using PackMeUp.Services.Interfaces;

namespace PackMeUp.ViewModels
{
    public class DocsViewModel : BaseViewModel
    {
        public DocsViewModel(ILocalUserService localUserService, ISupabaseService supabase, ISessionService sessionService, IPackingItemRepository packingItemRepository, ITripRepository tripRepository, IGoogleAuthService googleAuthService)
            : base(localUserService, supabase, sessionService, packingItemRepository, tripRepository, googleAuthService)
        {
        }
    }
}
