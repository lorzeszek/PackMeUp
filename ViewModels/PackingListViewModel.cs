using PackMeUp.Extensions;
using PackMeUp.Models;
using PackMeUp.Repositories.Enums;
using PackMeUp.Repositories.Interfaces;
using PackMeUp.Services.Interfaces;
using System.Windows.Input;

namespace PackMeUp.ViewModels
{
    public class PackingListViewModel : BaseViewModel
    {
        public ObservableRangeCollection<PackingItem> Items { get; } = new();

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

        public ICommand AddItemCommand => new Command(async () => await AddItemAsync());
        public ICommand ToggleIsPackedCommand => new Command<PackingItem>(async packingItem => await ToggleIsPackedAsync(packingItem));

        public PackingListViewModel(ISupabaseService supabase, ISessionService sessionService, IPackingItemRepository packingItemRepository, ITripRepository tripRepository) : base(supabase, sessionService, packingItemRepository, tripRepository)
        {
        }

        private async Task AddItemAsync()
        {
            IsBusy = true;
            IsRefreshing = true;

            var newItem = new PackingItem { Name = _newItemName, Category = 3, TripId = _tripId, User_id = Session.UserId };

            await _packingItemRepository.AddPackingItemAsync(newItem);

            NewItemName = string.Empty;

            IsBusy = false;
            IsRefreshing = false;
        }

        private async Task ToggleIsPackedAsync(PackingItem packingItem)
        {
            if (packingItem != null)
            {
                try
                {
                    await _packingItemRepository.UpdatePackingItemAsync(packingItem);
                }
                catch (Supabase.Postgrest.Exceptions.PostgrestException ex)
                {
                    // Logowanie pełnego wyjątku
                    Console.WriteLine($"Error: {ex.Message}, {ex.StackTrace}");
                }
            }
        }

        //private async Task<bool> CheckItemExist(string newItemName)
        //{
        //    var response = await _supabase.Client.From<PackingItem>().Where(x => x.TripId == _tripId).Get();

        //    if (response.Models == null || !response.Models.Any())
        //        return false;

        //    return response.Models.Any(x => x.Name == newItemName);
        //}

        public Task DisposeRealtimeAsync()
        {
            if (_subscription != null)
            {
                _subscription.Unsubscribe();
            }

            return Task.CompletedTask;
        }

        private void OnPackingItemChanged(PackingItemChange change)
        {
            if (change.Item.TripId != _tripId)
                return;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                switch (change.Type)
                {
                    case PackingItemChangeType.Insert:
                        Items.Add(change.Item);
                        break;

                    case PackingItemChangeType.Update:
                        var item = Items.FirstOrDefault(x => x.Id == change.Item.Id);
                        if (item != null)
                        {
                            var index = Items.IndexOf(item);

                            if (index >= 0)
                            {
                                Items[index] = change.Item;
                            }
                        }

                        break;

                    case PackingItemChangeType.Delete:
                        var toRemove = Items.FirstOrDefault(i => i.Id == change.Item.Id);
                        if (toRemove != null)
                            Items.Remove(toRemove);
                        break;
                }
            });
        }

        private async Task LoadData()
        {
            var tripsWithStats = await _packingItemRepository.GetPackingItemsForTripAsync(_tripId);

            Items.ReplaceRange(tripsWithStats);
        }

        protected override async Task OnNavigatedToAsync(IDictionary<string, object> query)
        {
            if (query.TryGetValue("tripId", out var tripIdObj))
            {
                _tripId = Convert.ToInt32(tripIdObj);

                if (!await _packingItemRepository.IsChannelCreatedAsync())
                {
                    await _packingItemRepository.StartRealtimeAsync();
                }

                _packingItemRepository.PackingItemChanged -= OnPackingItemChanged;
                _packingItemRepository.PackingItemChanged += OnPackingItemChanged;

                await LoadData();
            }
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> query)
        {
            _packingItemRepository.PackingItemChanged -= OnPackingItemChanged;

            await _packingItemRepository.SyncPendingChangesAsync();
        }
    }
}
