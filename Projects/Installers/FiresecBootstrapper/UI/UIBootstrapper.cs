using System.Windows.Threading;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;

namespace UI
{
	public class UIBootstrapper : BootstrapperApplication
	{
		static public Dispatcher BootstrapperDispatcher { get; private set; }
		
		protected override void Run()
		{
			this.Engine.Log(LogLevel.Verbose, "Launching custom TestBA UX");
			BootstrapperDispatcher = Dispatcher.CurrentDispatcher;

			MainViewModel viewModel = new MainViewModel(this);
			MainView view = new MainView();
			viewModel.Bootstrapper.Engine.Detect();
			view.DataContext = viewModel;
			view.Closed += (sender, e) => BootstrapperDispatcher.InvokeShutdown();
			view.Show();

			Dispatcher.Run();

			this.Engine.Quit(0);
		}
	}
}