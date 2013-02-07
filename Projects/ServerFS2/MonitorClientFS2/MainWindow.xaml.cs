using System.Windows;

namespace MonitorClientFS2
{
	public partial class MainWindow : Window
	{
		MainWindowViewModel mainWindowViewModel = new MainWindowViewModel();

		public MainWindow()
		{
			this.DataContext = mainWindowViewModel;
			InitializeComponent();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			var dataContext = this.DataContext as MainWindowViewModel;
			if (dataContext.StopMonitoringCommand.CanExecute(null))
				dataContext.StopMonitoringCommand.Execute();
		}
	}
}