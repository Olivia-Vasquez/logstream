using Microsoft.Extensions.Logging;
using LogStream.ViewModels;

using System.IO;
using Microsoft.Maui.Storage;
using LogStream.Services;

namespace LogStream;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		// Register Databases

		var dbPath = Path.Combine(FileSystem.AppDataDirectory, "LogsDB.db3");
		builder.Services.AddSingleton(new LogsDatabase(dbPath));


		// Register ViewModels
		builder.Services.AddTransient<MainPageViewModel>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
