using System.Windows.Controls;
using LightOnSpotApp.Pages;
using LightOnSpotApp.Services;

namespace LightOnSpotApp.ViewModels
{
    public class StartPageViewModel
    {
        private Page currentPage;
        public StartPageViewModel(Page currentPage)
        {
            this.currentPage = currentPage;
        }

        private RelayCommand createNewProjectCommand;
        public RelayCommand CreateNewProjectCommand
        {
            get
            {
                return createNewProjectCommand ??= new RelayCommand((obj) =>
                {
                    // TODO: open project template manager
                    MainFrameService.Instance.MainFrame.Navigate(new CreateProjectWithOptionsPage(currentPage));
                });
            }
        }

        private RelayCommand openProjectCommand;
        public RelayCommand OpenProjectCommand
        {
            get
            {
                return openProjectCommand ??= new RelayCommand((obj) =>
                {
                    // TODO: open project from file
                });
            }
        }
    }
}