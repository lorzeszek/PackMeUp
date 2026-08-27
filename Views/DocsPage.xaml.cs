using Packo.Models.Pages;
using Packo.ViewModels;

namespace Packo.Views
{
    public partial class DocsPage : BasePage
    {
        public DocsPage(DocsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
