namespace PackMeUp.Services.Interfaces
{
    public interface ISupabaseService
    {
        Supabase.Client Client { get; }

        Task InitializeAsync();
    }
}
