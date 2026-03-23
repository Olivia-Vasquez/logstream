using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogStream.Maui.Services;

namespace LogStream.Maui.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly IThemeService _themeService;

        public event EventHandler? CloseRequested;

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
            var current = _themeService.GetCurrentThemeMode();
            SelectedTheme = current;
            // Initialize radio buttons to reflect the current saved theme
            _isSystem = current == AppThemeMode.System;
            _isLight  = current == AppThemeMode.Light;
            _isDark   = current == AppThemeMode.Dark;
        }

        // Apply theme immediately when any radio button is checked
        partial void OnIsSystemChanged(bool value) { if (value) _themeService.SetThemeMode(AppThemeMode.System); }
        partial void OnIsLightChanged(bool value)  { if (value) _themeService.SetThemeMode(AppThemeMode.Light); }
        partial void OnIsDarkChanged(bool value)   { if (value) _themeService.SetThemeMode(AppThemeMode.Dark); }

        [RelayCommand]
        public void Close()
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
