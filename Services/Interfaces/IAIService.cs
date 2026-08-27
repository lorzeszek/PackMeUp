namespace Packo.Services.Interfaces
{
    public interface IAIService
    {
        Task<string> GetCompletionAsync(string prompt);
    }
}
