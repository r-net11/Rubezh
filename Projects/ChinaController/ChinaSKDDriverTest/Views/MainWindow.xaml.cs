using System.ComponentModel;
using System.Windows;
using ControllerSDK.ViewModels;
using StrazhDeviceSDK;

namespace ControllerSDK.Views
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			MainViewModel.Wrapper = new Wrapper();
			MainViewModel = new MainViewModel();
			DataContext = MainViewModel;
		}

		public MainViewModel MainViewModel { get; private set; }

		protected override void OnClosing(CancelEventArgs e)
		{
			MainViewModel.Wrapper.Disconnect();
			Wrapper.Deinitialize();
			ConnectionSettingsHelper.Set(new ConnectionSettings()
			{
				Address = MainViewModel.ConnectionViewModel.Address,
				Port = MainViewModel.ConnectionViewModel.Port,
				Login = MainViewModel.ConnectionViewModel.Login,
				Password = MainViewModel.ConnectionViewModel.Password
			});

			base.OnClosing(e);
		}
	}
}