using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using PlansModule.Events;

namespace PlansModule.ViewModels
{
    public class DevicesInZoneViewModel : SaveCancelDialogContent
    {
        public DevicesInZoneViewModel(Dictionary<Device, ulong?> deviceInZones)
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

        protected override void Save(ref bool cancel)
        {
            foreach (var deviceInZone in DeviceInZones)
            {
                if (deviceInZone.IsActive)
                {
                    deviceInZone.Activate();
                }
            }
        }
    }

    public class DeviceInZoneViewModel : BaseViewModel
    {
        public Device Device { get; private set; }
        public ulong? NewZoneNo { get; private set; }
        public string OldZoneName { get; private set; }
        public string NewZoneName { get; private set; }

        public DeviceInZoneViewModel(Device device, ulong? newZoneNo)
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