using System.Windows.Controls;
using LightOnSpotApp;
using LightOnSpotApp.Services;

namespace LightOnSpotApp.ViewModels
{
    public class CreateProjectWithOptionsViewModel
    {
        private Page currentPage;
        private Page parentPage;

        public CreateProjectWithOptionsViewModel(Page currentPage, Page parentPage)
        {
            this.currentPage = currentPage;
            this.parentPage = parentPage;
        }

        private RelayCommand backCommand;
        public RelayCommand BackCommand
        {
            get
            {
                return backCommand ??= new RelayCommand((e) =>
                {
                    // TODO: go to previous page.
                    MainFrameService.Instance.MainFrame.Navigate(parentPage);
                });
            }
        }

        private RelayCommand createNewProjectCommand;
        public RelayCommand CreateNewProjectCommand
        {
            get
            {
                return createNewProjectCommand ??= new RelayCommand((e) =>
                {
                    // TODO: create new project and open it.
                    //MainFrameService.Instance.MainFrame.Navigate(parentPage);
                });
            }
        }
    }
}