namespace PackMeUp.Services
{
    public interface ISupabaseService
    {
        Supabase.Client Client { get; }

        Task InitializeAsync(); // jeśli chcesz np. zrobić init klienta
    }
}
