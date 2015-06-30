using System.Windows;
using FiresecClient;
using GKProcessor;

namespace GKSDK
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
			GKProcessorManager.Stop();
            FiresecManager.Disconnect();
			//System.Environment.Exit(1);
        }
	}
}