using Android.App;
using Android.Content.PM;

namespace PackMeUp
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity// IPlatformActivity
    {
        //public Activity GetCurrentActivity() => this;


        //        var activity = Platform.CurrentActivity
        //                       ?? throw new Exception("No current activity");

        //        var credentialManager = CredentialManager.Create(activity);

        //        var googleIdOption = new GetGoogleIdOption.Builder()
        //            .SetFilterByAuthorizedAccounts(false)
        //            //.SetServerClientId(activity.GetString(Resource.String.server_client_id))
        //            .SetServerClientId("249767082828-93lqcq1afkb7ms7s3gosusr8e2f4sjmn.apps.googleusercontent.com")
        //            .Build();

        //        var request = new GetCredentialRequest.Builder()
        //            .AddCredentialOption(googleIdOption)
        //            //.AddOption(googleIdOption)
        //            .Build();

        //        try
        //        {
        //            //var result = await credentialManager.GetCredentialAsync(activity, request);
        //            var result = await credentialManager.GetCredentialAsync(activity.ApplicationContext, request);

        //            if (result.Credential is CustomCredential cred &&
        //                //cred.Type == GoogleIdTokenCredential.TypeGoogleIdToken)
        //                cred.Type == GoogleIdTokenCredential.TypeGoogleIdTokenCredential)
        //            {
        //                var tokenResult = GoogleIdTokenCredential.CreateFrom(cred.Data);

        //                return tokenResult.IdToken;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            System.Diagnostics.Debug.WriteLine(ex);
        //        }

        //        return null;
        //    }
        //}

        //public interface IPlatformActivity
        //{
        //    Activity GetCurrentActivity();
        //}
    }
}


