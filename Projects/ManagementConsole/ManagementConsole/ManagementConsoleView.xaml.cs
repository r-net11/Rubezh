using System.Windows;

namespace ManagementConsole
{
	public partial class ManagementConsoleView : Window
	{
		public ManagementConsoleView()
		{
			InitializeComponent();
			var ManagementConsoleViewModel = new ManagementConsoleViewModel();
			DataContext = ManagementConsoleViewModel;
		}
	}
}