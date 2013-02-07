using System.Windows;

namespace MonitorClientFS2
{
	public partial class MainView : Window
	{
		public MainView()
		{
			InitializeComponent();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			var dataContext = this.DataContext as MainViewModel;
			if (dataContext.StopMonitoringCommand.CanExecute(null))
				dataContext.StopMonitoringCommand.Execute();
		}
	}
}