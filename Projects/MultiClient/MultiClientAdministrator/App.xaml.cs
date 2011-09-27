using System.Windows;
using MultiClient.Services;

namespace MultiClient
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var shellView = new ShellView();

            var layoutService = new LayoutService();
            layoutService.Initialize(shellView);
            ServiceFactory.Layout = layoutService;
            ServiceFactory.UserDialogs = new UserDialogService();

            shellView.Show();
        }

    }
}
