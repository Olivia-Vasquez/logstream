using CommunityToolkit.Maui.Views;
using LogStream.Maui.ViewModels;

namespace LogStream.Maui.Views
{
    public partial class SettingsPopup : Popup
    {
        public SettingsPopup(SettingsViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
            vm.CloseRequested += async (_, _) => await CloseAsync();

            ApplyThemeColors();
            Application.Current!.RequestedThemeChanged += (_, _) => ApplyThemeColors();
        }

        private void ApplyThemeColors()
        {
            var app = Application.Current;
            // UserAppTheme reflects the app-level override; fall back to OS theme when set to System
            var effective = app?.UserAppTheme == AppTheme.Unspecified
                ? app?.RequestedTheme
                : app?.UserAppTheme;
            var isDark = effective == AppTheme.Dark;

            var bgColor = isDark ? Color.FromArgb("#23272E") : Color.FromArgb("#FAFAFA");

            // Sync Popup's own background so the padding area never shows white
            BackgroundColor = bgColor;
            PopupBorder.BackgroundColor = bgColor;
        }
    }
}
