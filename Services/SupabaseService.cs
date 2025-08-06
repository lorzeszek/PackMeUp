namespace PackMeUp.Services
{
    public class SupabaseService : ISupabaseService
    {
        private readonly Supabase.Client _client;

        public Supabase.Client Client => _client;

        public SupabaseService()
        {
            var url = "https://dahwafdelpczzbgvlelc.supabase.co";
            var apiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImRhaHdhZmRlbHBjenpiZ3ZsZWxjIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTQwNTgzMTYsImV4cCI6MjA2OTYzNDMxNn0.vVAFp0wKgaM5804ITEcAph8jp2G-laGOngdiY2Qkfl0";

            _client = new Supabase.Client(url, apiKey, new Supabase.SupabaseOptions
            {
                AutoConnectRealtime = true
            });
        }

        public async Task InitializeAsync()
        {
            await _client.InitializeAsync(); // bardzo ważne
        }
    }
}
