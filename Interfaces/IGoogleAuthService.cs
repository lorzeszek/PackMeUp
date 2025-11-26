namespace PackMeUp.Interfaces
{
    public interface IGoogleAuthService
    {
        Task<string?> SignInWithGoogleAsync();
    }
}
