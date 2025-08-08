using PackMeUp.Extensions;
using PackMeUp.Models;
using PackMeUp.Services;
using System.Windows.Input;

namespace PackMeUp.ViewModels
{
    public partial class PackingListViewModel : BaseViewModel
    {
        private bool _isSubscribed = false;
        private int _tripId { get; set; }

        public ObservableRangeCollection<PackingItem> Items { get; } = new();
        public ICommand AddItemCommand => new Command(async () => await Task.Run(() => AddItemAsync("new item")));

        public PackingListViewModel(ISupabaseService supabase) : base(supabase)
        {
        }

        public async Task LoadTripItemsAsync(string tripId)
        {
            _tripId = int.Parse(tripId);

            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var response = await _supabase.Client.From<PackingItem>().Where(x => x.TripId == _tripId).Get();

                var loadedItems = response.Models
                    .Select(x => new PackingItem
                    {
                        Id = x.Id,
                        TripId = x.TripId,
                        Name = x.Name,
                        IsPacked = x.IsPacked,
                        Category = x.Category,
                        CreatedDate = x.CreatedDate,
                        ModifiedDate = x.ModifiedDate,
                    })
                    .ToList();

                Items.ReplaceRange(loadedItems);
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        private async Task AddItemAsync(string newItemName)
        {
            if (!string.IsNullOrEmpty(newItemName))
            {
                var newItem = new PackingItem { Name = newItemName, Category = 3, TripId = _tripId };

                bool itemExists = await CheckItemExist(newItemName);

                if (itemExists)
                {
                    //if (itemExists.IsPacked)
                    //{
                    //    itemExists.IsPacked = false;
                    //}
                    ////else
                    ////{
                    ////    existingItem.ItemCount++;
                    ////}

                    //var updateResponse = await _supabase.Client
                    //.From<PackingItem>()
                    //.Update(itemExists);
                }
                else
                {
                    await _supabase.Client
                    .From<PackingItem>()
                    .Insert(newItem);
                }
            }
        }

        private async Task<bool> CheckItemExist(string newItemName)
        {
            try
            {
                var response = await _supabase.Client.From<PackingItem>().Where(x => x.TripId == _tripId).Get();

                if (response.Models == null || !response.Models.Any())
                    return false;

                return response.Models.Any(x => x.Name == newItemName);
            }
            catch (Exception ex)
            {
            }

            return false;
        }

        public async Task InitializeRealtimeAsync()
        {
            if (_supabase.Client == null)
                throw new Exception("Supabase Client is not initialized");

            if (!_isSubscribed)
            {
                await _supabase.Client
                    .From<PackingItem>()
                    .On(Supabase.Realtime.PostgresChanges.PostgresChangesOptions.ListenType.Inserts, async (sender, args) =>
                    {
                        var newItem = args.Model<PackingItem>();

                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            Items.Add(newItem);
                        });

                        await Task.CompletedTask;
                    });

                _isSubscribed = true;
            }

            IsBusy = true;

            try
            {
                var response = await _supabase.Client
                    .From<PackingItem>()
                    .Where(x => x.TripId == _tripId)
                    .Get();

                Items.ReplaceRange(response.Models);
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        protected override async Task OnNavigatedToAsync(IDictionary<string, object> query)
        {
            if (query.TryGetValue("tripId", out var tripIdObj))
            {
                _tripId = Convert.ToInt32(tripIdObj);
                await InitializeRealtimeAsync();
            }
        }
    }
}
