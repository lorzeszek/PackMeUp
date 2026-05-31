using PackMeUp.Services.Interfaces;

namespace PackMeUp.Services
{
    public class PackingSuggestionService : IPackingSuggestionService
    {
        private readonly IAIService _aiService;

        public PackingSuggestionService(IAIService aiService)
        {
            _aiService = aiService;
        }

        public async Task<List<string>> GenerateItemsAsync(string destination, DateTime start, DateTime end)
        {
            var days = (end - start).Days;

            var prompt = $@"
        Generate a packing list for a trip.

        Destination: {destination}
        Duration: {days} days

        Return ONLY a simple list of item names, one per line.
        ";

            var response = await _aiService.GetCompletionAsync(prompt);

            return Parse(response);
        }

        private List<string> Parse(string response)
        {
            return response
                .Split('\n')
                .Select(x => x.Trim('-', ' ', '\r'))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }
    }
}
