using System.Windows;

namespace DiagramDesigner
{    
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainViewModel mainViewModel = new MainViewModel();
            MainView mainView = new MainView();
            mainViewModel.MainView = mainView;
            mainView.DataContext = mainViewModel;
            mainViewModel.Initialize();
            mainView.Show();
        }
    }
}
