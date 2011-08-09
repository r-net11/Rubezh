using System.Windows;

namespace Logger
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            View view = new View();
            ViewModel viewModel = new ViewModel();
            view.DataContext = viewModel;
            view.Show();
        }
    }
}