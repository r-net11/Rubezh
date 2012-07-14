using System.Windows;

namespace ItvIntegration
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			var mainViewModel = new MainViewModel();
			DataContext = mainViewModel;
		}
	}
}