using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using RubezhClient;
using RubezhAPI;
using RubezhAPI.GK;

namespace GKModule.ViewModels
{
	public class DevicesInsertIntoViewModel : SaveCancelDialogViewModel
	{
		public DevicesInsertIntoViewModel(GKDevice device)
		{
			Title = "Переместить в";
			DeviceToCopy = device;
			RootDevice = AddDeviceInternal(GKManager.DeviceConfiguration.RootDevice, null);
			RootDevice.GetAllChildren().FindAll(x => x.Device.DriverType == GKDriverType.RSR2_KAU_Shleif).ForEach(y => y.ExpandToThis());
			if (DeviceToCopy != null)
				SelectedDevice = RootDevice.GetAllChildren().FirstOrDefault(x => x.Device.UID == DeviceToCopy.UID);
			if (SelectedDevice == null)
				SelectedDevice = RootDevice.GetAllChildren().FirstOrDefault();
			if (SelectedDevice != null)
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
			get { return new[] { RootDevice }; }
		}

		DeviceViewModel _selectedDevice;
		GKDevice DeviceToCopy { get; set; }
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
			if (SelectedDevice != null)
			{
				if (SelectedDevice.IsDisabled || SelectedDevice.GetAllParents().Any(x => x.IsDisabled))
					return false;

				if (SelectedDevice.Device.DriverType == GKDriverType.RSR2_KAU || SelectedDevice.Device.DriverType == GKDriverType.KAUIndicator)
					return false;

				if (SelectedDevice.Device.IsConnectedToKAU)
				{
					if (SelectedDevice.Parent == null)
						return false;

					if (!SelectedDevice.Driver.Children.Contains(DeviceToCopy.DriverType) && !SelectedDevice.Parent.Driver.Children.Contains(DeviceToCopy.DriverType))
						return false;
					return true;
				}
				else
				{
					if (!SelectedDevice.Driver.Children.Contains(DeviceToCopy.DriverType))
						return false;
					return true;
				}
			}
			return false;
		}
	}
}