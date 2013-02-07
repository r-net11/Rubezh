using System.Windows;

namespace MonitorClientFS2
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		MainWindowViewModel mainWindowViewModel = new MainWindowViewModel();

		public MainWindow()
		{
			this.DataContext = mainWindowViewModel;
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
		}

		private void Window_Unloaded(object sender, RoutedEventArgs e)
		{
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			var dataContext = this.DataContext as MainWindowViewModel;
			if (dataContext.StopMonitoringCommand.CanExecute(null))
				dataContext.StopMonitoringCommand.Execute();
		}
	}
}