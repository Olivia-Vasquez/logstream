using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogStream.Maui.Services;

namespace LogStream.Maui.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly IThemeService _themeService;

        [ObservableProperty]
        private AppThemeMode _selectedTheme;


        // radio button bindings
        [ObservableProperty]
        private bool _isSystem;
        [ObservableProperty]
        private bool _isLight;
        [ObservableProperty]
        private bool _isDark;

        // App version
        [ObservableProperty]
        private string _appVersion = "1.0.0";
        public SettingsViewModel(IThemeService themeService)
        {
            _themeService = themeService;
            SelectedTheme = _themeService.GetCurrentThemeMode();
        }

        [RelayCommand]
        public void SetTheme(AppThemeMode mode)
        {
            SelectedTheme = mode;
            _themeService.SetThemeMode(mode);
        }

        [RelayCommand]
        public void Close()
        {
            // Update theme based on selected radio button
            if (IsSystem)
            {
                SetTheme(AppThemeMode.System);
            }
            else if (IsLight)
            {
                SetTheme(AppThemeMode.Light);
            }
            else if (IsDark)
            {
                SetTheme(AppThemeMode.Dark);
            }

            // Close the popup
            Shell.Current.GoToAsync("..");
        }
    }
}
