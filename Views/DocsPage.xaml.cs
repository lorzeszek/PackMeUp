using Packo.Models.Pages;
using Packo.ViewModels;

namespace Packo.Views
{
    public partial class DocsPage : BasePage
    {
        private readonly DocsViewModel _viewModel;

        public DocsPage(DocsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.OnAppearingAsync();
        }
    }
}
