using PackMeUp.ViewModels;

namespace PackMeUp.Models.Pages
{
    public partial class BasePage : ContentPage
    {
        protected override async void OnNavigatedFrom(NavigatedFromEventArgs args)
        {
            if (BindingContext is BaseViewModel vm)
            {
                _ = vm.OnNavigatedFromAsync(new Dictionary<string, object>());
            }

            base.OnNavigatedFrom(args);
        }
    }
}
