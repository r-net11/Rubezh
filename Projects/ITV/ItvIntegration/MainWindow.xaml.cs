using System.Windows;
using FiresecClient.Itv;

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

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ItvManager.Disconnect();
        }
	}
}