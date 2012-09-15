using System.Collections.ObjectModel;
using DevicesModule.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace DiagnosticsModule.ViewModels
{
	public class DevicesTreeViewModel : DialogViewModel
	{
		public DevicesTreeViewModel()
		{
			Title = "Дерево устройств";
			BuildTree();
		}

		public ObservableCollection<DeviceViewModel> Devices { get; private set; }

		void BuildTree()
		{
			Devices = new ObservableCollection<DeviceViewModel>();
			var deviceViewModel = AddDevice(FiresecManager.FiresecConfiguration.DeviceConfiguration.RootDevice, null);
			Devices.Add(deviceViewModel);
		}

		public DeviceViewModel AddDevice(Device device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceViewModel(device, Devices);
			deviceViewModel.Parent = parentDeviceViewModel;

			foreach (var childDevice in device.Children)
			{
				var childDeviceViewModel = AddDevice(childDevice, deviceViewModel);
				deviceViewModel.Children.Add(childDeviceViewModel);
			}

			return deviceViewModel;
		}
	}
}