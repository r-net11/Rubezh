using System.Windows.Controls;
using DevicesModule.ViewModels;
using FiresecClient;

namespace DevicesModule.Views
{
	public partial class DevicesMenuView : UserControl
	{
		public DevicesMenuView()
		{
			InitializeComponent();
			Loaded += new System.Windows.RoutedEventHandler(DevicesMenuView_Loaded);
		}

		void DevicesMenuView_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			DevicesMenuViewModel devicesMenuViewModel = DataContext as DevicesMenuViewModel;
			if (devicesMenuViewModel != null && FiresecManager.IsFS2Enabled)
			{
				_additionalMenu.DataContext = devicesMenuViewModel.Context.FS2DeviceCommandsViewModel;
			}
		}
	}
}