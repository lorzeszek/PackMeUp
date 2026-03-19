using PackMeUp.Models.Pages;
using PackMeUp.ViewModels;

namespace PackMeUp.Views
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
