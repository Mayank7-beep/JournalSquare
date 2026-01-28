using Microsoft.Extensions.Logging;
using JournalSquare.Data;
using JournalSquare.Services;
using JournalSquare.Services.Analytics;

namespace JournalSquare;

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
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // SQLite database
        string dbPath = Path.Combine(
            FileSystem.AppDataDirectory,
            "journalsquare.db");

        builder.Services.AddSingleton(new AppDatabase(dbPath));

        // Backend services
        builder.Services.AddSingleton<JournalService>();
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<StreakService>();
        builder.Services.AddSingleton<AnalyticsService>();
        
      
        builder.Services.AddSingleton<UserService>();
        


        return builder.Build();
    }
}