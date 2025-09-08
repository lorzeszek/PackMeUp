using Supabase.Postgrest.Models;

namespace PackMeUp.Helpers
{
    public static class RealtimeSubscriptionHelper
    {
        public static async Task<List<Supabase.Realtime.RealtimeChannel>> SubscribeTableChanges<T>(
            Supabase.Client client,
            Action<T> onInsert,
            Action<T> onUpdate,
            Action<T> onDelete
        ) where T : BaseModel, new()
        {
            var table = client.From<T>();
            var channels = new List<Supabase.Realtime.RealtimeChannel>();

            var insertChannel = await table.On(Supabase.Realtime.PostgresChanges.PostgresChangesOptions.ListenType.Inserts, async (sender, args) =>
            {
                var newItem = args.Model<T>();
                if (newItem != null) onInsert?.Invoke(newItem);
                await Task.CompletedTask;
            });
            channels.Add(insertChannel);

            var updateChannel = await table.On(Supabase.Realtime.PostgresChanges.PostgresChangesOptions.ListenType.Updates, async (sender, args) =>
            {
                var updatedItem = args.Model<T>();
                if (updatedItem != null) onUpdate?.Invoke(updatedItem);
                await Task.CompletedTask;
            });
            channels.Add(updateChannel);

            var deleteChannel = await table.On(Supabase.Realtime.PostgresChanges.PostgresChangesOptions.ListenType.Deletes, async (sender, args) =>
            {
                var deletedItem = args.Model<T>();
                if (deletedItem != null) onDelete?.Invoke(deletedItem);
                await Task.CompletedTask;
            });
            channels.Add(deleteChannel);

            return channels;
        }

    }


}
