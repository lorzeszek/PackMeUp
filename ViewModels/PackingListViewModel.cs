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


        private string _newItemName;
        public string NewItemName
        {
            get => _newItemName;
            set
            {
                if (_newItemName != value)
                {
                    _newItemName = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableRangeCollection<PackingItem> Items { get; } = new();
        //public ICommand AddItemCommand => new Command<string>(async (newItemName) => await Task.Run(() => AddItemAsync(newItemName)));
        public ICommand AddItemCommand => new Command(async () => await Task.Run(() => AddItemAsync()));



        //public ICommand AddItemCommand => new Command<string>(async (newItemName) =>
        //{
        //    if (string.IsNullOrWhiteSpace(newItemName))
        //        return;

        //    await AddItemAsync(newItemName);
        //});

        public PackingListViewModel(ISupabaseService supabase) : base(supabase)
        {
        }


        protected override async Task ExecuteRefreshCommand()
        {
            await InitializeRealtimeAsync();
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

        private async Task AddItemAsync()
        {
            if (!string.IsNullOrEmpty(NewItemName))
            {
                IsBusy = true;
                IsRefreshing = true;

                var newItem = new PackingItem { Name = _newItemName, Category = 3, TripId = _tripId };

                try
                {
                    bool itemExists = await CheckItemExist(_newItemName);

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
                catch
                {

                }
                finally
                {
                    IsBusy = false;
                    IsRefreshing = false;
                }

                NewItemName = string.Empty;
            }
        }

        private async Task<bool> CheckItemExist(string newItemName)
        {
            var response = await _supabase.Client.From<PackingItem>().Where(x => x.TripId == _tripId).Get();

            if (response.Models == null || !response.Models.Any())
                return false;

            return response.Models.Any(x => x.Name == newItemName);
        }

        public async Task InitializeRealtimeAsync()
        {
            if (_supabase.Client == null)
                throw new Exception("Supabase Client is not initialized");
            try
            {
                IsBusy = true;
                IsRefreshing = true;

                if (!_isSubscribed || _subscription == null)
                {
                    _subscription = await _supabase.Client
                        .From<PackingItem>()
                        .On(Supabase.Realtime.PostgresChanges.PostgresChangesOptions.ListenType.Inserts, async (sender, args) =>
                        {
                            var newItem = args.Model<PackingItem>();

                            if (newItem != null)
                            {
                                MainThread.BeginInvokeOnMainThread(() =>
                                {
                                    Items.Add(newItem);
                                });

                                await Task.CompletedTask;
                            }
                        });

                    _isSubscribed = true;
                }

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

        public Task DisposeRealtimeAsync()
        {
            if (_subscription != null)
            {
                _subscription.Unsubscribe();
            }

            return Task.CompletedTask;
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
