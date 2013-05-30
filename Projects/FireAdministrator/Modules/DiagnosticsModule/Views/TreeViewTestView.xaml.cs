using System.Windows.Controls;
using DiagnosticsModule.ViewModels;
using System.Collections.Generic;
using FiresecClient;
using FiresecAPI.Models;
using DevicesModule.ViewModels;

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
				parentDeviceViewModel.Children.Add(deviceViewModel);

			foreach (var childDevice in device.Children)
				AddDeviceInternal(childDevice, deviceViewModel);
			return deviceViewModel;
		}
	}
}