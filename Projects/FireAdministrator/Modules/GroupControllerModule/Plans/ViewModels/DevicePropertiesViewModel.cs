using GKModule.ViewModels;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using System.Linq;

namespace GKModule.Plans.ViewModels
{
	public class DevicePropertiesViewModel : SaveCancelDialogViewModel
	{
		ElementGKDevice _elementGKDevice;

		public DevicePropertiesViewModel(ElementGKDevice elementDevice)
		{
			Title = "Свойства фигуры: Устройство ГК";
			_elementGKDevice = elementDevice;

			RootDevice = AddDeviceInternal(GKManager.DeviceConfiguration.RootDevice, null);
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
			if (device.UID == _elementGKDevice.DeviceUID)
				SelectedDevice = deviceViewModel;
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
		public string DeviceDescription
		{
			get
			{
				return SelectedDevice != null ? SelectedDevice.Description : "";
			}
			set
			{
				if (SelectedDevice != null)
				{
					SelectedDevice.Description = value;
					var device = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device == SelectedDevice.Device);
					if (device != null)
						device.Update();
				}
				OnPropertyChanged(() => DeviceDescription);
			}
		}

		protected override bool Save()
		{
			GKPlanExtension.Instance.RewriteItem(_elementGKDevice, SelectedDevice.Device);
			return base.Save();
		}

		protected override bool CanSave()
		{
			return SelectedDevice != null && SelectedDevice.Driver.IsPlaceable;
		}
	}
}