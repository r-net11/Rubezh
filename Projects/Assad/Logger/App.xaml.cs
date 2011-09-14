using System.Windows;

namespace Logger
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var view = new View();
            var viewModel = new ViewModel();
            view.DataContext = viewModel;
            view.Show();
        }
    }
}