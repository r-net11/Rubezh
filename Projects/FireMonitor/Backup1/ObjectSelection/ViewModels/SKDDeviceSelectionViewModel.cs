using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class SKDDeviceSelectionViewModel : SaveCancelDialogViewModel
	{
		public SKDDeviceSelectionViewModel(SKDDevice device)
		{
			Title = "Выбор устройства";
			RootDevice = AddDeviceInternal(SKDManager.SKDConfiguration.RootDevice, null);
			if (device != null)
				SelectedDevice = RootDevice.GetAllChildren().FirstOrDefault(x => x.SKDDevice.UID == device.UID);
			if (SelectedDevice == null)
				SelectedDevice = RootDevice.GetAllChildren().FirstOrDefault();
			if(SelectedDevice != null)
				SelectedDevice.ExpandToThis();
		}

		DeviceViewModel AddDeviceInternal(SKDDevice device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);

			foreach (var childDevice in device.Children)
				AddDeviceInternal(childDevice, deviceViewModel);
			return deviceViewModel;
		}

		DeviceViewModel _rootDevice;
		public DeviceViewModel RootDevice
		{
			get { return _rootDevice; }
			private set
			{
				_rootDevice = value;
				OnPropertyChanged(() => RootDevice);
				OnPropertyChanged(() => RootDevices);
			}
		}
		public DeviceViewModel[] RootDevices
		{
			get { return new DeviceViewModel[] { RootDevice }; }
		}

		DeviceViewModel _selectedDevice;
		public DeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged(() => SelectedDevice);
			}
		}

		protected override bool CanSave()
		{
			return ((SelectedDevice == null) || (SelectedDevice.SKDDevice.DriverType == SKDDriverType.Lock));
		}
	}
}