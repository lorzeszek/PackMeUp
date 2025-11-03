using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MPowerKit.Lottie;
using PackMeUp.Services;
using PackMeUp.ViewModels;
using PackMeUp.Views;

namespace PackMeUp
{
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
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
                })
                .UseMPowerKitLottie();



            builder.Services.AddSingleton<ISupabaseService, SupabaseService>();

            builder.Services.AddTransient<TripListPage>();
            builder.Services.AddTransient<PackingListPage>();

            builder.Services.AddTransient<TripListViewModel>();
            builder.Services.AddTransient<PackingListViewModel>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
