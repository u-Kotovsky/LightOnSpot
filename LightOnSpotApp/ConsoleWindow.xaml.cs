using System.Windows;
using LightOnSpotApp.ViewModels;

namespace LightOnSpotApp
{
    /// <summary>
    /// Interaction logic for ConsoleWindow.xaml
    /// </summary>
    public partial class ConsoleWindow : Window
    {
        private ConsoleViewModel viewModel;

        public ConsoleWindow(ConsoleViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = this.viewModel;
        }
    }
}
