using CommunityToolkit.Maui.Views;
using PackMeUp.Popups.Objects;
using System.Collections.ObjectModel;

namespace PackMeUp.Popups;

public partial class PackingItemsPopup : Popup
{
    public ObservableCollection<SelectableItem> Items { get; set; }

    public TaskCompletionSource<List<string>> ResultSource { get; } = new();

    public PackingItemsPopup(List<string> items)
    {
        InitializeComponent();

        Items = new ObservableCollection<SelectableItem>(
            items.Select(x => new SelectableItem
            {
                Name = x,
                IsSelected = true
            }));

        BindingContext = this;
    }

    private void OnSelectAllClicked(object sender, EventArgs e)
    {
        foreach (var item in Items)
            item.IsSelected = true;
    }

    private void OnDeselectAllClicked(object sender, EventArgs e)
    {
        foreach (var item in Items)
            item.IsSelected = false;
    }

    private async void OnConfirmClicked(object sender, EventArgs e)
    {
        var selected = Items
            .Where(x => x.IsSelected)
            .Select(x => x.Name)
            .ToList();

        await CloseAsync();
        ResultSource.TrySetResult(selected);
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await CloseAsync();
        ResultSource.TrySetResult([]);
    }
}