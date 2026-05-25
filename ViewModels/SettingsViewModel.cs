using CommunityToolkit.Mvvm.ComponentModel;
using PackMeUp.Helpers;
using PackMeUp.Interfaces;
using PackMeUp.Repositories.Interfaces;
using PackMeUp.Services.Interfaces;

namespace PackMeUp.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    public IReadOnlyList<string> ThemeOptions { get; } = ["System", "Light", "Dark"];

    [ObservableProperty]
    private string selectedTheme = "System";

    public SettingsViewModel(ILocalUserService localUserService, ISupabaseService supabase, ISessionService sessionService, IPackingItemRepository packingItemRepository, ITripRepository tripRepository, IGoogleAuthService googleAuthService) : base(localUserService, supabase, sessionService, packingItemRepository, tripRepository, googleAuthService)
    {
        SelectedTheme = AppThemeManager.GetSavedTheme();
        AppThemeManager.ApplyTheme(SelectedTheme);
    }

    partial void OnSelectedThemeChanged(string value)
    {
        AppThemeManager.SaveTheme(value);
        AppThemeManager.ApplyTheme(value);
    }
}
