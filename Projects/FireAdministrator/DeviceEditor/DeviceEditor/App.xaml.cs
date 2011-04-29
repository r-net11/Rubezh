using System.Windows;

namespace DeviceEditor
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
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