using System.Windows.Threading;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;

namespace FiresecBootstrapperApplication
{
    public class FiresecBootstrapperApplication : BootstrapperApplication
    {
		public static Dispatcher BootstrapperDispatcher { get; private set; }
		
		protected override void Run()
        {
            this.Engine.Log(LogLevel.Verbose, "Launching Firesec Bootstrapper Application UX");
            BootstrapperDispatcher = Dispatcher.CurrentDispatcher;
			MainView view = new MainView();
			view.DataContext = new MainViewModel(this);
            view.Closed += (sender, e) => BootstrapperDispatcher.InvokeShutdown();
            view.Show();
			Dispatcher.Run();
			this.Engine.Quit(0);
        }
    }
}
