using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class DirectionDeviceSelectorViewModel : SaveCancelDialogViewModel
	{
		public DirectionDeviceSelectorViewModel(Direction direction, DriverType driverType)
		{
			Title = "Выбор устройства";

			var devices = new HashSet<Device>();

			foreach (var device in FiresecManager.Devices)
			{
				if (device.Driver.DriverType == driverType)
				{
					if (device.Parent.Children.Any(x => x.Driver.IsZoneDevice && x.ZoneUID != Guid.Empty && direction.ZoneUIDs.Contains(x.ZoneUID)))
					{
						device.AllParents.ForEach(x => { devices.Add(x); });
						devices.Add(device);
					}
				}
			}

			Devices = new ObservableCollection<DeviceViewModel>();
			foreach (var device in devices)
			{
				var deviceViewModel = new DeviceViewModel(device);
				deviceViewModel.IsExpanded = true;
				Devices.Add(deviceViewModel);
			}

			foreach (var device in Devices.Where(x => x.Device.Parent != null))
			{
				var parent = Devices.FirstOrDefault(x => x.Device.UID == device.Device.Parent.UID);
				parent.AddChild(device);
			}

			SelectedDevice = Devices.FirstOrDefault(x => x.HasChildren == false);
		}

		public ObservableCollection<DeviceViewModel> Devices { get; private set; }

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
			if (SelectedDevice != null)
				return SelectedDevice.HasChildren == false;
			return false;
		}
	}
}