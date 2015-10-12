using System.Windows;
using RubezhClient;
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
            ClientManager.Disconnect();
			//System.Environment.Exit(1);
        }
	}
}