using LogStream.Maui.Services;

namespace LogStream.Maui;

public partial class App : Application
{
    private readonly IServiceProvider _services;

    public App(IServiceProvider services, IThemeService themeService)
    {
        InitializeComponent();

        _services = services;
        themeService.InitializeTheme();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(_services.GetRequiredService<AppShell>());
    }
}