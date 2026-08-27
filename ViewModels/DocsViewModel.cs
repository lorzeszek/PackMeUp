using Packo.Interfaces;
using Packo.Repositories.Interfaces;
using Packo.Services.Interfaces;

namespace Packo.ViewModels
{
    public class DocsViewModel : BaseViewModel
    {
        public DocsViewModel(ILocalUserService localUserService, ISupabaseService supabase, ISessionService sessionService, IPackingItemRepository packingItemRepository, ITripRepository tripRepository, IGoogleAuthService googleAuthService)
            : base(localUserService, supabase, sessionService, packingItemRepository, tripRepository, googleAuthService)
        {
        }
    }
}
