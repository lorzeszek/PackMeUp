using PackMeUp.ViewModels;

namespace PackMeUp.Views
{
    public partial class ShellHeaderView : ContentView
    {
        public ShellHeaderView()
        {
            InitializeComponent();

            // Resolve ViewModel from DI container
            if (Application.Current?.Handler?.MauiContext?.Services != null)
            {
                var viewModel = Application.Current.Handler.MauiContext.Services.GetService<ShellHeaderViewModel>();
                if (viewModel != null)
                {
                    BindingContext = viewModel;
                }
            }
        }

        public ShellHeaderView(ShellHeaderViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
