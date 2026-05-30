namespace PackMeUp.Services.Interfaces
{
    public interface IPackingSuggestionService
    {
        Task<List<string>> GenerateItemsAsync(string destination, DateTime start, DateTime end, int adultsCount, List<int> childrenCountWithAge, List<string> selectedTransportTypes);
    }
}
