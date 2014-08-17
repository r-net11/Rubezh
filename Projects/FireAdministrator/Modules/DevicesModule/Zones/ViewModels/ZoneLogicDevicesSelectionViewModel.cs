using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class ZoneLogicDevicesSelectionViewModel : SaveCancelDialogViewModel
	{
		Device Device;
		public List<Device> SelectedDevices { get; private set; }
		ZoneLogicState ZoneLogicState;

		public ZoneLogicDevicesSelectionViewModel(Device device, List<Device> selectedDevices, ZoneLogicState zoneLogicState)
		{
			Title = "Выбор устройств";
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);

			Device = device;
			ZoneLogicState = zoneLogicState;
			SelectedDevices = selectedDevices;
			InitializeDevices();
			InitializeAvailableDevices();
		}

		void InitializeDevices()
		{
			Devices = new ObservableCollection<DeviceViewModel>();
			foreach (var device in SelectedDevices)
			{
				var deviceViewModel = new DeviceViewModel(device);
				Devices.Add(deviceViewModel);
			}
			SelectedDevice = Devices.FirstOrDefault();
		}

		void InitializeAvailableDevices()
		{
			var devices = new HashSet<Device>();

			foreach (var device in FiresecManager.Devices)
			{
				if (Devices.Any(x => x.Device.UID == device.UID))
					continue;

				if (ZoneLogicState == ZoneLogicState.AM1TOn && (device.Driver.DriverType == DriverType.AM1_T || device.Driver.DriverType == DriverType.MDU || device.Driver.DriverType == DriverType.ShuzOffButton || device.Driver.DriverType == DriverType.ShuzOnButton || device.Driver.DriverType == DriverType.ShuzUnblockButton))
				{
					if (device.ParentPanel != null && device.ParentPanel.UID == Device.ParentPanel.UID)
					{
						device.AllParents.ForEach(x => { devices.Add(x); });
						devices.Add(device);
					}
				}
				if (ZoneLogicState == ZoneLogicState.ShuzOn && device.Driver.DriverType == DriverType.Valve)
				{
					if (device.ParentPanel != null && device.ParentPanel.UID == Device.ParentPanel.UID)
					{
						device.AllParents.ForEach(x => { devices.Add(x); });
						devices.Add(device);
					}
				}
			}

			AvailableDevices = new ObservableCollection<DeviceViewModel>();
			foreach (var device in devices)
			{
				var deviceViewModel = new DeviceViewModel(device);
				deviceViewModel.IsExpanded = true;
				AvailableDevices.Add(deviceViewModel);
			}

			foreach (var device in AvailableDevices)
			{
				if (device.Device.Parent != null)
				{
					var parent = AvailableDevices.FirstOrDefault(x => x.Device.UID == device.Device.Parent.UID);
					parent.AddChild(device);
				}
			}

			SelectedAvailableDevice = AvailableDevices.FirstOrDefault(x => x.ChildrenCount == 0);
		}

		ObservableCollection<DeviceViewModel> _devices;
		public ObservableCollection<DeviceViewModel> Devices
		{
			get { return _devices; }
			set
			{
				_devices = value;
				OnPropertyChanged(() => Devices);
			}
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

		ObservableCollection<DeviceViewModel> _availableDevices;
		public ObservableCollection<DeviceViewModel> AvailableDevices
		{
			get { return _availableDevices; }
			set
			{
				_availableDevices = value;
				OnPropertyChanged(() => AvailableDevices);
			}
		}

		DeviceViewModel _selectedAvailableDevice;
		public DeviceViewModel SelectedAvailableDevice
		{
			get { return _selectedAvailableDevice; }
			set
			{
				_selectedAvailableDevice = value;
				OnPropertyChanged(() => SelectedAvailableDevice);
			}
		}

		bool CanAdd()
		{
			if (SelectedAvailableDevice != null && SelectedAvailableDevice.HasChildren == false)
			{
				return true;
			}
			return false;
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var deviceViewModel = new DeviceViewModel(SelectedAvailableDevice.Device);
			Devices.Add(deviceViewModel);
			SelectedDevices.Add(SelectedAvailableDevice.Device);
			InitializeAvailableDevices();
		}

		bool CanRemove()
		{
			if (SelectedDevice != null)
				return true;
			return false;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			SelectedDevices.Remove(SelectedDevice.Device);
			Devices.Remove(SelectedDevice);
			InitializeAvailableDevices();
		}

		protected override bool Save()
		{
			return true;
		}
	}
}