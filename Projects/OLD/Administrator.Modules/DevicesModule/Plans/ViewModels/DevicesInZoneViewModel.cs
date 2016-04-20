using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace DevicesModule.Plans.ViewModels
{
	public class DevicesInZoneViewModel : SaveCancelDialogViewModel
	{
		public DevicesInZoneViewModel(Dictionary<Device, Guid> deviceInZones)
		{
			Title = "Изменение зон устройств на плане";
			DeviceInZones = new List<DeviceInZoneViewModel>();
			foreach (var x in deviceInZones.ToList())
			{
				var deviceInZoneViewModel = new DeviceInZoneViewModel(x.Key, x.Value);
				DeviceInZones.Add(deviceInZoneViewModel);
			}
		}

		public List<DeviceInZoneViewModel> DeviceInZones { get; private set; }

		protected override bool Save()
		{
			foreach (var deviceInZone in DeviceInZones)
			{
				if (deviceInZone.IsActive)
				{
					deviceInZone.Activate();
				}
			}
			return base.Save();
		}
	}

	public class DeviceInZoneViewModel : BaseViewModel
	{
		public Device Device { get; private set; }
		public Guid NewZoneUID { get; private set; }
		public string OldZoneName { get; private set; }
		public string NewZoneName { get; private set; }

		public DeviceInZoneViewModel(Device device, Guid newZoneUID)
		{
			IsActive = true;
			Device = device;
			NewZoneUID = newZoneUID;

			if (NewZoneUID != Guid.Empty)
			{
				var newZone = FiresecManager.Zones.FirstOrDefault(x => x.UID == NewZoneUID);
				NewZoneName = newZone.PresentationName;
			}
			else
			{
				NewZoneName = "";
			}

			var zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == Device.ZoneUID);
			OldZoneName = zone.PresentationName;
		}

		public void Activate()
		{
			var zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == NewZoneUID);
			if (zone != null)
			{
				FiresecManager.FiresecConfiguration.AddDeviceToZone(Device, zone);
			}
			else
			{
				FiresecManager.FiresecConfiguration.AddDeviceToZone(Device, null);
			}
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