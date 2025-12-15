using PackMeUp.Extensions;
using PackMeUp.Helpers;
using PackMeUp.Services.Interfaces;

namespace PackMeUp.ViewModels
{
    public abstract class RealtimeViewModel<TModel, TViewModel> : BaseViewModel
        where TModel : Supabase.Postgrest.Models.BaseModel, new()
    {
        protected bool _isSubscribed;

        protected RealtimeViewModel(ISupabaseService supabase, ISessionService sessionService) : base(supabase, sessionService)
        {
        }

        protected abstract ObservableRangeCollection<TViewModel> ItemsCollection { get; }

        protected abstract TViewModel MapToViewModel(TModel model);

        protected virtual bool ShouldAdd(TModel model) => true;
        protected virtual bool ShouldRemove(TModel model) => false;

        protected abstract object GetId(TModel model);
        protected abstract object GetModelId(TViewModel vm);

        public async Task InitializeRealtimeAsync(Func<Task<IEnumerable<TModel>>> fetchInitialData)
        {
            if (_supabase.Client == null)
                throw new Exception("Supabase Client is not initialized");

            try
            {
                IsBusy = true;
                IsRefreshing = true;

                if (!_isSubscribed)
                {
                    var subs = await RealtimeSubscriptionHelper.SubscribeTableChanges<TModel>(
                        _supabase.Client,
                        newItem =>
                        {
                            if (newItem != null && ShouldAdd(newItem))
                                MainThread.BeginInvokeOnMainThread(() => ItemsCollection.Add(MapToViewModel(newItem)));
                        },
                        updatedItem =>
                        {
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                var existing = ItemsCollection.FirstOrDefault(x => GetModelId(x).Equals(GetId(updatedItem)));
                                if (existing != null)
                                {
                                    if (ShouldRemove(updatedItem))
                                        ItemsCollection.Remove(existing);
                                    else
                                    {
                                        var i = ItemsCollection.IndexOf(existing);
                                        ItemsCollection[i] = MapToViewModel(updatedItem);
                                    }
                                }
                                else if (!ShouldRemove(updatedItem))
                                {
                                    ItemsCollection.Add(MapToViewModel(updatedItem));
                                }
                            });
                        },
                        deletedItem =>
                        {
                            var existing = ItemsCollection.FirstOrDefault(x => GetModelId(x).Equals(GetId(deletedItem)));
                            if (existing != null)
                                MainThread.BeginInvokeOnMainThread(() => ItemsCollection.Remove(existing));
                        });

                    _isSubscribed = true;
                    _subscription = subs.FirstOrDefault();
                }

                var initialData = await fetchInitialData();
                var vmList = initialData.Select(MapToViewModel).ToList();
                MainThread.BeginInvokeOnMainThread(() => ItemsCollection.ReplaceRange(vmList));
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }
    }

}
