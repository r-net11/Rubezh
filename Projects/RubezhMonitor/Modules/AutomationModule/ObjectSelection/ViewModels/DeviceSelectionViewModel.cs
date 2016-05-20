using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;

namespace AutomationModule.ViewModels
{
	public class DeviceSelectionViewModel : SaveCancelDialogViewModel
	{
		public DeviceSelectionViewModel(GKDevice device)
		{
			Title = "Выбор устройства";
			RootDevice = AddDeviceInternal(GKManager.DeviceConfiguration.RootDevice, null);
			if (device != null)
				SelectedDevice = RootDevice.GetAllChildren().FirstOrDefault(x => x.Device.UID == device.UID);
			if (SelectedDevice == null)
				SelectedDevice = RootDevice.GetAllChildren().FirstOrDefault();
			if(SelectedDevice != null)
				SelectedDevice.ExpandToThis();
		}

		DeviceViewModel AddDeviceInternal(GKDevice device, DeviceViewModel parentDeviceViewModel)
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
			return ((SelectedDevice == null) || ((SelectedDevice.Device.Driver.IsControlDevice) || (SelectedDevice.Device.Driver.IsDeviceOnShleif)));
		}
	}
}