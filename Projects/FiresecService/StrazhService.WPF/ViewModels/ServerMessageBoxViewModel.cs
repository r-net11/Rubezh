using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System.Windows;

namespace StrazhService.ViewModels
{
	public class ServerMessageBoxViewModel : MessageBoxViewModel
	{
		public ServerMessageBoxViewModel(string title, string message, MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage, bool isException = false)
			: base(title, message, messageBoxButton, messageBoxImage, isException)
		{
		}

		public override void OnLoad()
		{
			Surface.Owner = ApplicationService.ApplicationWindow;
			Surface.ShowInTaskbar = false;
			Surface.WindowStartupLocation = WindowStartupLocation.CenterScreen;
		}

		public override int GetPreferedMonitor()
		{
			return MonitorHelper.FindMonitor(ApplicationService.ApplicationWindow.RestoreBounds);
		}
	}
}