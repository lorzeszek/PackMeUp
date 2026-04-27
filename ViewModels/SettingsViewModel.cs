using CommunityToolkit.Mvvm.ComponentModel;
using PackMeUp.Interfaces;
using PackMeUp.Repositories.Interfaces;
using PackMeUp.Services.Interfaces;

namespace PackMeUp.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    private const string ThemePreferenceKey = "app_theme";

    public IReadOnlyList<string> ThemeOptions { get; } = ["System", "Light", "Dark"];

    [ObservableProperty]
    private string selectedTheme = "System";

    public SettingsViewModel(ILocalUserService localUserService, ISupabaseService supabase, ISessionService sessionService, IPackingItemRepository packingItemRepository, ITripRepository tripRepository, IGoogleAuthService googleAuthService) : base(localUserService, supabase, sessionService, packingItemRepository, tripRepository, googleAuthService)
    {
        SelectedTheme = LoadThemePreference();
        ApplyTheme(SelectedTheme);
    }

    partial void OnSelectedThemeChanged(string value)
    {
        SaveThemePreference(value);
        ApplyTheme(value);
    }

    private static void ApplyTheme(string theme)
    {
        if (Application.Current is null)
            return;

        Application.Current.UserAppTheme = theme switch
        {
            "Light" => AppTheme.Light,
            "Dark" => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };
    }

    private static string LoadThemePreference()
    {
        return Preferences.Get(ThemePreferenceKey, "System");
    }

    private static void SaveThemePreference(string theme)
    {
        Preferences.Set(ThemePreferenceKey, theme);
    }
}
