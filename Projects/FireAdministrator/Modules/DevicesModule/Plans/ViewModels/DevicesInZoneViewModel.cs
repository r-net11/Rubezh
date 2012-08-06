using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using DevicesModule.Plans.Events;

namespace DevicesModule.Plans.ViewModels
{
	public class DevicesInZoneViewModel : SaveCancelDialogViewModel
    {
		public DevicesInZoneViewModel(Dictionary<Device, int?> deviceInZones)
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
		public int? NewZoneNo { get; private set; }
        public string OldZoneName { get; private set; }
        public string NewZoneName { get; private set; }

		public DeviceInZoneViewModel(Device device, int? newZoneNo)
        {
            IsActive = true;
            Device = device;
            NewZoneNo = newZoneNo;

            if (newZoneNo.HasValue)
            {
                var newZone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == newZoneNo.Value);
                NewZoneName = newZone.PresentationName;
            }
            else
            {
                NewZoneName = "";
            }

            var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == Device.ZoneNo);
            OldZoneName = zone.PresentationName;
        }

        public void Activate()
        {
            Device.ZoneNo = NewZoneNo;
            ServiceFactory.Events.GetEvent<DeviceInZoneChangedEvent>().Publish(Device.UID);
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