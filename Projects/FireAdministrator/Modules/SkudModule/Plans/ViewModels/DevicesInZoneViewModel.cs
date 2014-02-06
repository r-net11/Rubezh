using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.Plans.ViewModels
{
	public class DevicesInZoneViewModel : SaveCancelDialogViewModel
	{
		public DevicesInZoneViewModel(Dictionary<SKDDevice, Guid> deviceInZones)
		{
			Title = "Изменение зон устройств на плане";
			var map = new Dictionary<Guid, SKDZone>();
			SKDManager.Zones.ForEach(item => map.Add(item.UID, item));
			DeviceInZones = new List<DeviceInZoneViewModel>();
			foreach (var x in deviceInZones.ToList())
			{
				var deviceInZoneViewModel = new DeviceInZoneViewModel(map, x.Key, x.Value);
				DeviceInZones.Add(deviceInZoneViewModel);
			}
		}

		public List<DeviceInZoneViewModel> DeviceInZones { get; private set; }

		protected override bool Save()
		{
			foreach (var deviceInZone in DeviceInZones)
				if (deviceInZone.IsActive)
					deviceInZone.Activate();
			return base.Save();
		}
	}

	public class DeviceInZoneViewModel : BaseViewModel
	{
		private Dictionary<Guid, SKDZone> _zoneMap;
		public SKDDevice Device { get; private set; }
		public Guid NewZoneUID { get; private set; }
		public string OldZoneName { get; private set; }
		public string NewZoneName { get; private set; }

		public DeviceInZoneViewModel(Dictionary<Guid, SKDZone> zoneMap, SKDDevice device, Guid newZoneUID)
		{
			_zoneMap = zoneMap;
			IsActive = true;
			Device = device;
			NewZoneUID = newZoneUID;
			var zone = NewZoneUID != Guid.Empty && _zoneMap.ContainsKey(NewZoneUID) ? _zoneMap[NewZoneUID] : null;
			NewZoneName = zone == null ? string.Empty : zone.Name;
			zone = Device.ZoneUID != Guid.Empty && _zoneMap.ContainsKey(Device.ZoneUID) ? _zoneMap[Device.ZoneUID] : null;
			OldZoneName = zone == null ? string.Empty : zone.Name;
		}

		public void Activate()
		{
			var zone = Device.ZoneUID != Guid.Empty && _zoneMap.ContainsKey(Device.ZoneUID) ? _zoneMap[Device.ZoneUID] : null;
			if (zone != null)
				SKDManager.RemoveDeviceFromZone(Device, zone);
			zone = _zoneMap.ContainsKey(NewZoneUID) ? _zoneMap[NewZoneUID] : null;
			if (zone != null)
				SKDManager.AddDeviceToZone(Device, zone);
		}

		public bool HasNewZone
		{
			get { return !string.IsNullOrEmpty(NewZoneName); }
		}

		public bool HasOldZone
		{
			get { return !string.IsNullOrEmpty(OldZoneName); }
		}

		bool _isActive;
		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				_isActive = value;
				OnPropertyChanged("IsActive");
			}
		}
	}
}