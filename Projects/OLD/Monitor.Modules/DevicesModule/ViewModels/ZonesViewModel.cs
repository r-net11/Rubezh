using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class ZonesViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public void Initialize()
		{
			Zones = (from Zone zone in FiresecManager.Zones
					 orderby zone.No
					 select new ZoneViewModel(zone.ZoneState)).ToList();
			SelectedZone = Zones.FirstOrDefault();
		}

		List<ZoneViewModel> _zones;
		public List<ZoneViewModel> Zones
		{
			get { return _zones; }
			set
			{
				_zones = value;
				OnPropertyChanged(() => Zones);
			}
		}

		ZoneViewModel _selectedZone;
		public ZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				if (value != null)
				{
					InitializeDevices();
				}
				OnPropertyChanged(() => SelectedZone);
			}
		}

		public void Select(Guid zoneUID)
		{
			if (zoneUID != Guid.Empty)
				SelectedZone = Zones.FirstOrDefault(x => x.Zone.UID == zoneUID);
		}

		public void InitializeDevices()
		{
			BuildTree();
			if (RootDevice != null)
			{
				RootDevice.IsExpanded = true;
			}
			OnPropertyChanged(() => RootDevices);
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

		DeviceViewModel _rootDevice;
		public DeviceViewModel RootDevice
		{
			get { return _rootDevice; }
			private set
			{
				_rootDevice = value;
				OnPropertyChanged(() => RootDevice);
			}
		}

		public DeviceViewModel[] RootDevices
		{
			get { return RootDevice == null ? null : new DeviceViewModel[] { RootDevice }; }
		}

		void BuildTree()
		{
			RootDevice = AddDeviceInternal(FiresecManager.FiresecConfiguration.DeviceConfiguration.RootDevice, null);
		}

		private DeviceViewModel AddDeviceInternal(Device device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceViewModel(device);
			deviceViewModel.IsExpanded = true;
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);

			foreach (var childDevice in device.Children)
			{
				if (SelectedZone.Zone.DevicesInZone.Any(x => x.AllParents.Contains(childDevice)) || SelectedZone.Zone.DevicesInZone.Any(x => x == childDevice))
				{
					AddDeviceInternal(childDevice, deviceViewModel);
				}
			}
			return deviceViewModel;
		}
	}
}