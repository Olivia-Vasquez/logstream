namespace LogStream.Maui.Services
{
    public enum AppThemeMode { System, Light, Dark }

    public interface IThemeService
    {
        AppThemeMode GetCurrentThemeMode();
        void SetThemeMode(AppThemeMode mode);
        void ApplyTheme(AppThemeMode mode);
        void InitializeTheme();
    }
}
