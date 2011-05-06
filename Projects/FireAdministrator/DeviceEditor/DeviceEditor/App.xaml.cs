using System.Windows;
using DeviceEditor.ViewModels;
using DeviceEditor.Views;

namespace DeviceEditor
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            var view = new View();
            var viewModel = new ViewModel();
            view.DataContext = viewModel;
            view.Show();
        }
    }
}