using PackMeUp.Extensions;
using PackMeUp.Models;
using PackMeUp.Services.Interfaces;
using System.Windows.Input;

namespace PackMeUp.ViewModels
{
    public class PackingListViewModel : RealtimeViewModel<PackingItem, PackingItem>
    {
        public ObservableRangeCollection<PackingItem> Items { get; } = new();

        protected override ObservableRangeCollection<PackingItem> ItemsCollection => Items;

        protected override PackingItem MapToViewModel(PackingItem model) => model;

        protected override object GetId(PackingItem model) => model.Id;
        protected override object GetModelId(PackingItem vm) => vm.Id;

        protected override bool ShouldAdd(PackingItem model) => true;

        //private bool _isSubscribed = false;
        private int _tripId { get; set; }


        private string _newItemName = string.Empty;
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

        //public ObservableRangeCollection<PackingItem> Items { get; } = new();
        public ICommand AddItemCommand => new Command(async () => await Task.Run(() => AddItemAsync()));
        public ICommand ToggleIsPackedCommand => new Command<PackingItem>(async (packingItem) => await Task.Run(() => ToggleIsPackedAsync(packingItem)));

        public PackingListViewModel(ISupabaseService supabase, ISessionService sessionService) : base(supabase, sessionService)
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

        private async Task AddItemAsync()
        {
            if (!string.IsNullOrEmpty(NewItemName))
            {
                IsBusy = true;
                IsRefreshing = true;

                var newItem = new PackingItem { Name = _newItemName, Category = 3, TripId = _tripId, User_id = Session.UserId };

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

        private async Task ToggleIsPackedAsync(PackingItem packingItem)
        {
            if (packingItem != null)
            {
                try
                {
                    var getItemResult = await _supabase.Client.From<PackingItem>().Where(x => x.Id == packingItem.Id).Get();

                    var selectedItem = getItemResult.Models.First();

                    selectedItem.IsPacked = packingItem.IsPacked;

                    var updateResponse = await _supabase.Client
                    .From<PackingItem>()
                    .Update(selectedItem);

                    //await InitializeRealtimeAsync();
                }
                catch (Supabase.Postgrest.Exceptions.PostgrestException ex)
                {
                    // Logowanie pełnego wyjątku
                    Console.WriteLine($"Error: {ex.Message}, {ex.StackTrace}");
                }
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
            await InitializeRealtimeAsync(async () =>
            {
                var response = await _supabase.Client
                    .From<PackingItem>()
                    .Where(x => x.TripId == _tripId)
                    .Get();

                return response.Models;
            });
        }
        //public async Task InitializeRealtimeAsync()
        //{
        //    if (_supabase.Client == null)
        //        throw new Exception("Supabase Client is not initialized");

        //    try
        //    {
        //        IsBusy = true;
        //        IsRefreshing = true;

        //        if (!_isSubscribed)// || _subscription == null)
        //        {
        //            var d = await RealtimeSubscriptionHelper.SubscribeTableChanges<PackingItem>(
        //                _supabase.Client,
        //                // INSERT handler
        //                newItem =>
        //                {
        //                    if (newItem != null)
        //                    {
        //                        MainThread.BeginInvokeOnMainThread(() => Items.Add(newItem));
        //                    }
        //                },
        //                // UPDATE handler
        //                updatedItem =>
        //                {
        //                    var existing = Items.FirstOrDefault(t => t.Id == updatedItem.Id);
        //                    MainThread.BeginInvokeOnMainThread(() =>
        //                    {
        //                        if (existing != null)
        //                        {
        //                            var i = Items.IndexOf(existing);
        //                            Items[i] = updatedItem;
        //                        }
        //                        else { Items.Add(updatedItem); }
        //                    });
        //                },
        //                // DELETE handler
        //                deletedItem =>
        //                {
        //                    var existing = Items.FirstOrDefault(t => t.Id == deletedItem.Id);
        //                    if (existing != null)
        //                    {
        //                        MainThread.BeginInvokeOnMainThread(() => Items.Remove(existing));
        //                    }
        //                }
        //            );

        //            _isSubscribed = true;

        //            _subscription = d.FirstOrDefault();
        //        }

        //        // 🔹 Początkowe pobranie z filtrem
        //        var response = await _supabase.Client
        //            .From<PackingItem>()
        //            .Where(x => x.TripId == _tripId)
        //            .Get();

        //        Items.ReplaceRange(response.Models);
        //    }
        //    finally
        //    {
        //        IsBusy = false;
        //        IsRefreshing = false;
        //    }
        //}


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

            await InitializeRealtimeAsync();
        }
    }
}
