using CommunityToolkit.Maui.Views;

namespace PackMeUp.Popups;

public partial class ConfirmPopup : Popup<bool>
{
    public ConfirmPopup(string title, string message)
    {
        InitializeComponent();
        TitleLabel.Text = title;
        MessageLabel.Text = message;
    }

    private async void OnConfirmClicked(object sender, EventArgs e)
    {
        await CloseAsync(true);
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await CloseAsync(false);
    }
}
