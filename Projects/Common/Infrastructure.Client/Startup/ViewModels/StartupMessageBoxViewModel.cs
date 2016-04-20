using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using System.Windows;

namespace Infrastructure.Client.Startup.ViewModels
{
	public class StartupMessageBoxViewModel : MessageBoxViewModel
	{
		public StartupMessageBoxViewModel(string title, string message, MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage, bool isException = false)
			: base(title, message, messageBoxButton, messageBoxImage, isException)
		{
		}

		public override void OnLoad()
		{
			if (StartupService.Instance.IsActive)
			{
				Surface.Owner = StartupService.Instance.OwnerWindow;
				Surface.ShowInTaskbar = false;
				Surface.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			}
			else
				base.OnLoad();
		}
	}
}