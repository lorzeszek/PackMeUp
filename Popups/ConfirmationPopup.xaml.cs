using UXDivers.Popups.Maui;
using UXDivers.Popups.Services;

namespace PackMeUp.Popups;

public partial class ConfirmationPopup : PopupResultPage<bool>
{
    public ConfirmationPopup()
    {
        InitializeComponent();
    }

    public override void OnNavigatedTo(IReadOnlyDictionary<string, object?> parameters)
    {
        base.OnNavigatedTo(parameters);

        if (parameters.TryGetValue("message", out var message))
        {
            MessageLabel.Text = message?.ToString() ?? "Are you sure?";
        }
    }

    private async void OnConfirmClicked(object sender, EventArgs e)
    {
        SetResult(true);
        await IPopupService.Current.PopAsync(this);
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        SetResult(false);
        await IPopupService.Current.PopAsync(this);
    }
}