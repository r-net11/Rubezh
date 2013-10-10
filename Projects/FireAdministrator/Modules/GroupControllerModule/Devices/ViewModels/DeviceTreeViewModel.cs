using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Linq;

namespace GKModule.ViewModels
{
	public class DeviceTreeViewModel:BaseViewModel
	{
		public DeviceTreeViewModel(XDevice device)
		{
			Devices = new ObservableCollection<DeviceViewModel>();
			InitializeDevices(device, null);
			if (Devices.Count > 0)
			{
				ExpandChild(Devices[0]);
				SelectedDevice = Devices[0];
			}
		}
		DeviceViewModel InitializeDevices(XDevice device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceViewModel(device);
			var indexOf = Devices.IndexOf(parentDeviceViewModel);
			Devices.Insert(indexOf + 1, deviceViewModel);
			foreach (var childDevice in device.Children)
			{
				var childDeviceViewModel = InitializeDevices(childDevice, deviceViewModel);
				deviceViewModel.AddChild(childDeviceViewModel);
			}
			return deviceViewModel;
		}
		static void ExpandChild(DeviceViewModel parentDeviceViewModel)
		{
			parentDeviceViewModel.IsExpanded = true;
			foreach (var deviceViewModel in parentDeviceViewModel.Children)
			{
				ExpandChild(deviceViewModel);
			}
		}
		public ObservableCollection<DeviceViewModel> Devices { get; set; }
		DeviceViewModel _selectedDevice;
		public DeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged("SelectedDevice");
			}
		}
	}
}
