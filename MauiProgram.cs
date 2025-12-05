using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MPowerKit.Lottie;
using PackMeUp.Interfaces;
using PackMeUp.Services;
using PackMeUp.Services.Interfaces;
using PackMeUp.ViewModels;
using PackMeUp.Views;
using Syncfusion.Maui.Core.Hosting;
using UXDivers.Popups.Maui;

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
                .ConfigureSyncfusionCore()
                .UseUXDiversPopups()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
                })
                .UseMPowerKitLottie();

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjGyl/Vkd+XU9FcVRDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS3tSd0RrWHpccndWR2BaUE91Xg==");

            builder.Services.AddSingleton<ISupabaseService, SupabaseService>();
            builder.Services.AddSingleton<ISessionService, SessionService>();

            builder.Services.AddTransient<StartPage>();
            builder.Services.AddTransient<TripListPage>();
            builder.Services.AddTransient<PackingListPage>();

            builder.Services.AddTransient<StartViewModel>();
            builder.Services.AddTransient<TripListViewModel>();
            builder.Services.AddTransient<PackingListViewModel>();

#if ANDROID
            builder.Services.AddSingleton<IGoogleAuthService, PackMeUp.Platforms.Android.GoogleAuthService>();
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
