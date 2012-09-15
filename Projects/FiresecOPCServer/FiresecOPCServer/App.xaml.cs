using System.Windows;

namespace FiresecOPCServer
{
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            Bootstrapper.Run();
        }
    }
}