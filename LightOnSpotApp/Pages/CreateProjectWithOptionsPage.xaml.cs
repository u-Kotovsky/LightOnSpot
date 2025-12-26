using System.Windows.Controls;
using LightOnSpotApp.ViewModels;

namespace LightOnSpotApp.Pages
{
    /// <summary>
    /// Interaction logic for CreateProjectWithOptionsPage.xaml
    /// </summary>
    public partial class CreateProjectWithOptionsPage : Page
    {
        private CreateProjectWithOptionsViewModel viewModel;
        private Page parentPage;

        public CreateProjectWithOptionsPage(Page parentPage)
        {
            InitializeComponent();
            this.parentPage = parentPage;
            viewModel = new CreateProjectWithOptionsViewModel(this, parentPage);
        }
    }
}
