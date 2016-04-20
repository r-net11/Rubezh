using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GKModule.Plans.ViewModels
{
	public class DevicesInZoneViewModel : SaveCancelDialogViewModel
	{
		public DevicesInZoneViewModel(Dictionary<GKDevice, Guid> deviceInZones)
		{
			Title = "Изменение зон устройств на плане";
			var map = new Dictionary<Guid, GKZone>();
			GKManager.Zones.ForEach(item => map.Add(item.UID, item));
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
			{
				if (deviceInZone.IsActive)
					deviceInZone.Activate();
				deviceInZone.Device.AllowBeOutsideZone = deviceInZone.AllowBeOutsideZone;
			}
			return base.Save();
		}
	}

	public class DeviceInZoneViewModel : BaseViewModel
	{
		private Dictionary<Guid, GKZone> _zoneMap;
		public GKDevice Device { get; private set; }
		public Guid NewZoneUID { get; private set; }
		public string OldZoneName { get; private set; }
		public string NewZoneName { get; private set; }

		public DeviceInZoneViewModel(Dictionary<Guid, GKZone> zoneMap, GKDevice device, Guid newZoneUID)
		{
			_zoneMap = zoneMap;
			IsActive = true;
			Device = device;
			NewZoneUID = newZoneUID;
			var zone = NewZoneUID != Guid.Empty && _zoneMap.ContainsKey(NewZoneUID) ? _zoneMap[NewZoneUID] : null;
			NewZoneName = zone == null ? string.Empty : zone.PresentationName;
			zone = Device.ZoneUIDs.Count == 1 && _zoneMap.ContainsKey(Device.ZoneUIDs[0]) ? _zoneMap[Device.ZoneUIDs[0]] : null;
			OldZoneName = zone == null ? string.Empty : zone.PresentationName;
			AllowBeOutsideZone = device.AllowBeOutsideZone;
		}

		public void Activate()
		{
			var zone = Device.ZoneUIDs.Count == 1 && _zoneMap.ContainsKey(Device.ZoneUIDs[0]) ? _zoneMap[Device.ZoneUIDs[0]] : null;
			if (zone != null)
				GKManager.RemoveDeviceFromZone(Device, zone);
			zone = _zoneMap.ContainsKey(NewZoneUID) ? _zoneMap[NewZoneUID] : null;
			if (zone != null)
				GKManager.AddDeviceToZone(Device, zone);
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
				if (_isActive)
					AllowBeOutsideZone = false;
				OnPropertyChanged(() => IsActive);
			}
		}
		bool _allowBeOutsideZone;
		public bool AllowBeOutsideZone
		{
			get { return _allowBeOutsideZone; }
			set
			{
				_allowBeOutsideZone = value;
				if (_allowBeOutsideZone)
					IsActive = false;
				OnPropertyChanged(() => AllowBeOutsideZone);
			}
		}
	}
}