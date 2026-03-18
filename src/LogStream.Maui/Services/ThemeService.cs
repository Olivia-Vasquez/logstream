using Microsoft.Maui.Storage;
using Application = Microsoft.Maui.Controls.Application;

namespace LogStream.Maui.Services
{
    public class ThemeService : IThemeService
    {
        private const string ThemeKey = "app_theme_mode";

        public AppThemeMode GetCurrentThemeMode()
        {
            var value = Preferences.Get(ThemeKey, nameof(AppThemeMode.System));
            return Enum.TryParse<AppThemeMode>(value, out var mode) ? mode : AppThemeMode.System;
        }

        public void SetThemeMode(AppThemeMode mode)
        {
            Preferences.Set(ThemeKey, mode.ToString());
            ApplyTheme(mode);
        }

        public void ApplyTheme(AppThemeMode mode)
        {
            if (Application.Current == null)
            {
                Console.WriteLine("Cannot apply theme: Application.Current is null.");
                return;
            }
            App.Current.UserAppTheme = mode switch
            {
                AppThemeMode.Light => AppTheme.Light,
                AppThemeMode.Dark => AppTheme.Dark,
                AppThemeMode.System => AppTheme.Unspecified,
                _ => AppTheme.Unspecified // Default to system if unknown value
            };
        }

        public void InitializeTheme()
        {
            ApplyTheme(GetCurrentThemeMode());
        }
    }
}
