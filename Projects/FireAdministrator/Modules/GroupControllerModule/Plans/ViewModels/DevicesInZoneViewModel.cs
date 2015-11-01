using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.Plans.ViewModels
{
	public class DevicesInZoneViewModel : SaveCancelDialogViewModel
	{
		public DevicesInZoneViewModel(Dictionary<GKDevice, Guid> deviceInZones)
		{
			Title = "Изменение зон устройств на плане";
			var zoneMap = new Dictionary<Guid, GKZone>();
			var guardZoneMap = new Dictionary<Guid, GKGuardZone>();
			GKManager.Zones.ForEach(item => zoneMap.Add(item.UID, item));
			GKManager.GuardZones.ForEach(item => guardZoneMap.Add(item.UID, item));
			DeviceInZones = new List<DeviceInZoneViewModel>();
			foreach (var x in deviceInZones.ToList())
			{
				var deviceInZoneViewModel = new DeviceInZoneViewModel(zoneMap, guardZoneMap, x.Key, x.Value);
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
		Dictionary<Guid, GKZone> _zoneMap;
		Dictionary<Guid, GKGuardZone> _guardZoneMap;
		public GKDevice Device { get; private set; }
		public Guid NewZoneUID { get; private set; }
		public string OldZoneName { get; private set; }
		public string NewZoneName { get; private set; }

		public DeviceInZoneViewModel(Dictionary<Guid, GKZone> zoneMap, Dictionary<Guid, GKGuardZone> guardZoneMap, GKDevice device, Guid newZoneUID)
		{
			_zoneMap = zoneMap;
			_guardZoneMap = guardZoneMap;
			IsActive = true;
			Device = device;
			NewZoneUID = newZoneUID;
			var newZoneName = NewZoneUID != Guid.Empty && _zoneMap.ContainsKey(NewZoneUID) ? _zoneMap[NewZoneUID].PresentationName : null;
			if(newZoneName == null)
				newZoneName = NewZoneUID != Guid.Empty && _guardZoneMap.ContainsKey(NewZoneUID) ? _guardZoneMap[NewZoneUID].PresentationName : null;
			NewZoneName = newZoneName;
			var oldZoneName = Device.ZoneUIDs.Count == 1 && _zoneMap.ContainsKey(Device.ZoneUIDs[0]) ? _zoneMap[Device.ZoneUIDs[0]].PresentationName : null;
			if (oldZoneName == null)
				oldZoneName = Device.GuardZoneUIDs.Count == 1  && _guardZoneMap.ContainsKey(Device.GuardZoneUIDs[0]) ? _guardZoneMap[Device.GuardZoneUIDs[0]].PresentationName : null;
			OldZoneName = oldZoneName;
		}

		public void Activate()
		{
			var zone = Device.ZoneUIDs.Count == 1 && _zoneMap.ContainsKey(Device.ZoneUIDs[0]) ? _zoneMap[Device.ZoneUIDs[0]] : null;
			if (zone != null)
				GKManager.RemoveDeviceFromZone(Device, zone);
			zone = _zoneMap.ContainsKey(NewZoneUID) ? _zoneMap[NewZoneUID] : null;
			if (zone != null)
				GKManager.AddDeviceToZone(Device, zone);

			var guardZone = Device.GuardZoneUIDs.Count == 1 && _guardZoneMap.ContainsKey(Device.GuardZoneUIDs[0]) ? _guardZoneMap[Device.GuardZoneUIDs[0]] : null;
			if (guardZone != null)
				GKManager.RemoveDeviceFromGuardZone(Device, guardZone);
			guardZone = _guardZoneMap.ContainsKey(NewZoneUID) ? _guardZoneMap[NewZoneUID] : null;
			if (guardZone != null)
				GKManager.AddDeviceToGuardZone(Device, guardZone);
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
				OnPropertyChanged(() => IsActive);
			}
		}
	}
}