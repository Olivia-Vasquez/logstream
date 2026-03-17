using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.Maui.Storage;
using CommunityToolkit.Maui;
using LogStream.Maui.Services;
using LogStream.Maui.ViewModels;
using LogStream.Maui.Views;
using LogStream.Core.Services;

namespace LogStream.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
		// Register services and view models
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "LogsDB.db3");

        builder.Services.AddSingleton(new LogsDatabase(dbPath));

        builder.Services.AddSingleton<Core.Abstractions.ILogRepository, SqliteLogRepository>(sp =>
        {
            var db = sp.GetRequiredService<LogsDatabase>();
            return new SqliteLogRepository(db);
        });

        builder.Services.AddSingleton<IThemeService, ThemeService>();

        builder.Services.AddSingleton<MainPageViewModel>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<AppShell>(sp =>
        {
            var mainPage = sp.GetRequiredService<MainPage>();
            return new AppShell(mainPage);
        });

        builder.Services.AddTransientPopup<SettingsPopup, SettingsViewModel>();

// Add debug logging in development mode
#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}