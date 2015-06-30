using System.Windows;

namespace AutoHosting
{
	public partial class View : Window
	{
		public View()
		{
			InitializeComponent();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			((sender as View).DataContext as ViewModel).StopCommand.Execute();
		}
	}
}