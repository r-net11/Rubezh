using System;
using System.Collections.ObjectModel;
using System.Linq;
using StrazhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class ZonesViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public void Initialize()
		{
			Zones = new ObservableCollection<ZoneViewModel>();
			foreach (var zone in SKDManager.Zones.OrderBy(x => x.No))
			{
				var zoneViewModel = new ZoneViewModel(zone);
				Zones.Add(zoneViewModel);
			}
			SelectedZone = Zones.FirstOrDefault();
		}

		ObservableCollection<ZoneViewModel> _zones;
		public ObservableCollection<ZoneViewModel> Zones
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
				OnPropertyChanged(() => SelectedZone);
				InitializeDevices();
			}
		}

		public void Select(Guid zoneUID)
		{
			if (zoneUID != Guid.Empty)
			{
				SelectedZone = Zones.FirstOrDefault(x => x.Zone.UID == zoneUID);
			}
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

		void InitializeDevices()
		{
			Devices = new ObservableCollection<DeviceViewModel>();
			if (SelectedZone == null)
				return;
			foreach (var device in SKDManager.Devices)
			{
				if (device.Driver.HasZone)
				{
					if (device.ZoneUID == SelectedZone.Zone.UID)
					{
						var deviceViewModel = new DeviceViewModel(device);
						Devices.Add(deviceViewModel);
					}
				}
			}
		}
	}
}