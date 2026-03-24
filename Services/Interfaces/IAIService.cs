namespace PackMeUp.Services.Interfaces
{
    public interface IAIService
    {
        Task<string> GetCompletionAsync(string prompt);
    }
}
