using PackMeUp.Models;
using PackMeUp.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PackMeUp.ViewModels
{
    public class PackingListViewModel : BaseViewModel
    {
        //public Trip Trip { get; }
        //public ObservableCollection<PackingItem> Items { get; set; }

        //public ICommand AddItemCommand { get; }

        //public PackingListViewModel(Trip trip)
        //{
        //    Title = $"Lista pakowania: {trip.Name}";
        //    Trip = trip;
        //    Items = new ObservableCollection<PackingItem>(trip.Items);
        //    AddItemCommand = new Command(AddItem);
        //}

        //private void AddItem()
        //{
        //    var item = new PackingItem { Name = "Nowy przedmiot", Category = Category.Other };
        //    Items.Add(item);
        //    Trip.Items.Add(item);
        //}
        //private Trip trip;
        //public Trip Trip
        //{
        //    get => trip;
        //    set => SetProperty(ref trip, value);
        //}

        private int _tripId { get; set; }


        private ObservableCollection<PackingItem> _items = new();
        public ObservableCollection<PackingItem> Items { get => _items; set { SetProperty(ref _items, value); } }

        public ICommand AddItemCommand { get; }

        //private readonly ITripService _tripService;

        //public PackingListViewModel(ITripService tripService)
        //{
        //    _tripService = tripService;
        //    AddItemCommand = new Command(AddItem);
        //}

        //public PackingListViewModel(ITripService tripService)
        //{
        //    _tripService = tripService;
        //    AddItemCommand = new Command(AddItem);
        //}

        public PackingListViewModel(ISupabaseService supabase) : base(supabase)
        {
            AddItemCommand = new Command(async () => await Task.Run(() => AddItemAsync("new item")));
        }

        //public PackingListViewModel(Guid id)
        //{
        //    //_tripService = tripService;
        //    AddItemCommand = new Command(AddItem);
        //}

        public async Task LoadTripItemsAsync(string tripId)
        {
            _tripId = int.Parse(tripId);

            if (IsBusy) return;

            try
            {
                IsBusy = true;

                //var response = await _supabase.Client.From<PackingItem>().Select("*").Get();
                var response = await _supabase.Client.From<PackingItem>().Where(x => x.TripId == _tripId).Get();

                if (response.Models.Count != 0)
                {
                    Items.Clear();

                    Items = new ObservableCollection<PackingItem>();

                    response.Models.ForEach(x => Items.Add(new PackingItem
                    {
                        Id = x.Id,
                        TripId = x.TripId,
                        Name = x.Name,
                        IsPacked = x.IsPacked,
                        Category = x.Category,
                        CreatedDate = x.CreatedDate,
                        ModifiedDate = x.ModifiedDate,
                    }));
                }
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }

            //IsBusy = true;
            //try
            //{
            //    //Trip = await _tripService.GetTripByIdAsync(tripId);

            //    Trip = new Trip { Id = int.Parse(tripId), Name = "Przykładowa wycieczka" }; // tymczasowe dane
            //    Items.Clear();
            //    //foreach (var item in Trip.Items)
            //    //    Items.Add(item);

            //    Items.Add(new PackingItem { Name = "Przykładowy przedmiot 1", Category = 1 });
            //    Items.Add(new PackingItem { Name = "Przykładowy przedmiot 2", Category = 2 });

            //    Title = $"Lista pakowania: {Trip.Name}";
            //}
            //finally
            //{
            //    IsBusy = false;
            //}
        }

        //private void AddItem()
        //{
        //    var item = new PackingItem { Name = "Nowy przedmiot", Category = 3 };
        //    Items.Add(item);
        //    //Trip.Items.Add(item);
        //    // tutaj możesz też od razu zapisać zmiany do bazy
        //}

        private async Task AddItemAsync(string newItemName)
        {
            if (!string.IsNullOrEmpty(newItemName))
            {
                var newItem = new PackingItem { Name = "Nowy przedmiot", Category = 3, TripId = _tripId };

                PackingItem? existingItem = await CheckItemExist(newItemName);

                if (existingItem != null)
                {
                    if (existingItem.IsPacked)
                    {
                        existingItem.IsPacked = false;
                    }
                    //else
                    //{
                    //    existingItem.ItemCount++;
                    //}

                    var updateResponse = await _supabase.Client
                    .From<PackingItem>()
                    .Update(existingItem);
                }
                else
                {
                    var response = await _supabase.Client
                    .From<PackingItem>()
                    .Insert(newItem);
                }

                //await LoadListItemsAsync(selectedShoppingListId);

                //MainThread.BeginInvokeOnMainThread(async () =>
                //{
                //    await LoadListItemsAsync(selectedShoppingListId);
                //});
            }
        }

        private async Task<PackingItem?> CheckItemExist(string newItemName)
        {
            try
            {
                var response = await _supabase.Client.From<PackingItem>().Where(x => x.TripId == _tripId).Get();

                return response?.Models?.FirstOrDefault(x => x.Name == newItemName);
            }
            catch (Exception ex)
            {
            }

            return null;
        }
    }
}
