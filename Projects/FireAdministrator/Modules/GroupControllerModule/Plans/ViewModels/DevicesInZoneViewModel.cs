using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.Plans.ViewModels
{
	public class DevicesInZoneViewModel : SaveCancelDialogViewModel
	{
		public DevicesInZoneViewModel(Dictionary<XDevice, Guid> deviceInZones)
		{
			Title = "Изменение зон устройств на плане";
			var map = new Dictionary<Guid, XZone>();
			XManager.Zones.ForEach(item => map.Add(item.UID, item));
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
		private Dictionary<Guid, XZone> _zoneMap;
		public XDevice Device { get; private set; }
		public Guid NewZoneUID { get; private set; }
		public string OldZoneName { get; private set; }
		public string NewZoneName { get; private set; }

		public DeviceInZoneViewModel(Dictionary<Guid, XZone> zoneMap, XDevice device, Guid newZoneUID)
		{
			_zoneMap = zoneMap;
			IsActive = true;
			Device = device;
			NewZoneUID = newZoneUID;
			var zone = NewZoneUID != Guid.Empty && _zoneMap.ContainsKey(NewZoneUID) ? _zoneMap[NewZoneUID] : null;
			NewZoneName = zone == null ? string.Empty : zone.PresentationName;
			zone = Device.ZoneUIDs.Count == 1 && _zoneMap.ContainsKey(Device.ZoneUIDs[0]) ? _zoneMap[Device.ZoneUIDs[0]] : null;
			OldZoneName = zone == null ? string.Empty : zone.PresentationName;
		}

		public void Activate()
		{
			var zone = Device.ZoneUIDs.Count == 1 && _zoneMap.ContainsKey(Device.ZoneUIDs[0]) ? _zoneMap[Device.ZoneUIDs[0]] : null;
			if (zone != null)
				XManager.RemoveDeviceFromZone(Device, zone);
			zone = _zoneMap.ContainsKey(NewZoneUID) ? _zoneMap[NewZoneUID] : null;
			if (zone != null)
				XManager.AddDeviceToZone(Device, zone);
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