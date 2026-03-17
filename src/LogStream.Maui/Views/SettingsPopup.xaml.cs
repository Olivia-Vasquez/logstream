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
        }
    }
}
