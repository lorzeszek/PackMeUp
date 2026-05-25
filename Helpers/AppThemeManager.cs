namespace PackMeUp.Helpers;

public static class AppThemeManager
{
    private const string ThemePreferenceKey = "app_theme";
    private const string DefaultTheme = "System";

    public static string GetSavedTheme()
    {
        return Preferences.Get(ThemePreferenceKey, DefaultTheme);
    }

    public static void SaveTheme(string theme)
    {
        Preferences.Set(ThemePreferenceKey, theme);
    }

    public static void ApplyTheme(string theme)
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

    public static void ApplySavedTheme()
    {
        ApplyTheme(GetSavedTheme());
    }
}
