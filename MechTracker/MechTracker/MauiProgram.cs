using MechTracker.Constants;
using MechTracker.Data;
using MechTracker.Services;
using MechTracker.Views;
using Microsoft.Extensions.Logging;
using SQLite;

namespace MechTracker
{
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

            // Register services
            string dbPath = MechConstants.GetDatabasePath();
            builder.Services.AddSingleton(new SQLiteAsyncConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create));

            builder.Services.AddSingleton<MechRepository>();

            builder.Services.AddSingleton<MechService>();
            builder.Services.AddTransient<CreateMechPage>();
            builder.Services.AddTransient<WeaponDamageInputPage>();
            builder.Services.AddTransient<SetArmorPage>();
            builder.Services.AddTransient<SetInternalsPage>();
            builder.Services.AddTransient<SelectPhasePage>();
            builder.Services.AddTransient<SelectActiveMechPage>();
            builder.Services.AddTransient<LoadMechPage>();
            builder.Services.AddSingleton<IUserPromptService, UserPromptService>();
            // ... other service registrations ...

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
