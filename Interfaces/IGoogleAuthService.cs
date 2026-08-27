namespace Packo.Interfaces
{
    public interface IGoogleAuthService
    {
        Task<string?> SignInWithGoogleAsync();
    }
}
