using System.Windows.Controls;
using DevicesModule.ViewModels;
using FiresecAPI.Models;
using FiresecClient;

namespace DiagnosticsModule.Views
{
	public partial class TreeViewTestView : UserControl
	{
		public TreeViewTestView()
		{
			InitializeComponent();
			_tree.Source = new DeviceViewModel[] { AddDeviceInternal(FiresecManager.FiresecConfiguration.DeviceConfiguration.RootDevice, null) };
		}
		private DeviceViewModel AddDeviceInternal(Device device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);

			foreach (var childDevice in device.Children)
				AddDeviceInternal(childDevice, deviceViewModel);
			return deviceViewModel;
		}
	}
}