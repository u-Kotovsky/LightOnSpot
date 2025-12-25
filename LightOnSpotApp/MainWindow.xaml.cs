using System.Windows;

namespace LightOnSpotApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Hide();
            var console = new ConsoleWindow(new ViewModels.ConsoleViewModel());
            console.Show();
            Close();
        }
    }
}