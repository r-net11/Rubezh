using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using Infrastructure;
using System.Collections.ObjectModel;
using FiresecClient;
using DevicesModule.Events;
using Infrastructure.Events;
using FiresecAPI.Models;

namespace DevicesModule.ViewModels
{
    public class ZonesViewModel : RegionViewModel
    {
        public ZonesViewModel()
        {
            ServiceFactory.Events.GetEvent<ZoneSelectedEvent>().Subscribe(Select);
            ServiceFactory.Events.GetEvent<ZoneStateChangedEvent>().Subscribe(OnZoneStateChanged);
            ServiceFactory.Events.GetEvent<DeviceStateChangedEvent>().Subscribe(OnDeviceStateChanged);
        }

        public void Initialize()
        {
            FiresecEventSubscriber.ZoneStateChangedEvent += new Action<string>(OnZoneStateChangedEvent);
        }

        public IEnumerable<ZoneViewModel> Zones
        {
            get
            {
                return from Zone zone in FiresecManager.Configuration.Zones
                       orderby Convert.ToInt32(zone.No)
                       select new ZoneViewModel(zone);
            }
        }

        ZoneViewModel _selectedZone;
        public ZoneViewModel SelectedZone
        {
            get { return _selectedZone; }
            set
            {
                _selectedZone = value;
                InitializeDevices();
                OnPropertyChanged("SelectedZone");
            }
        }

        ObservableCollection<DeviceViewModel> _devices;
        public ObservableCollection<DeviceViewModel> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                OnPropertyChanged("Devices");
            }
        }

        void OnZoneStateChanged(string zoneNo)
        {
            ZoneState zoneState = FiresecManager.States.ZoneStates.FirstOrDefault(x => x.No == zoneNo);
            if (zoneState != null)
            {
                ZoneViewModel zoneViewModel = Zones.FirstOrDefault(x => x.Zone.No == zoneNo);
                if (zoneViewModel != null)
                {
                    zoneViewModel.State = zoneState.State;
                }
            }
        }

        void OnDeviceStateChanged(string id)
        {
            DeviceState deviceState = FiresecManager.States.DeviceStates.FirstOrDefault(x => x.Id == id);
            if (deviceState != null)
            {
                if (Devices != null)
                {
                    DeviceViewModel deviceViewModel = Devices.FirstOrDefault(x => x.Device.Id == id);
                    if (deviceViewModel != null)
                    {
                        deviceViewModel.Update();
                        //deviceViewModel.MainState = deviceState.State;
                    }
                }
            }
        }

        void OnZoneStateChangedEvent(string zoneNo)
        {
            ZoneState zoneState = FiresecManager.States.ZoneStates.FirstOrDefault(x => x.No == zoneNo);
            ZoneViewModel zoneViewModel = Zones.FirstOrDefault(x => x.Zone.No == zoneNo);
            zoneViewModel.State = zoneState.State;
        }

        public void Select(string zoneNo)
        {
            SelectedZone = Zones.FirstOrDefault(x => x.Zone.No == zoneNo);
        }


        void InitializeDevices()
        {
            if (SelectedZone == null)
                return;

            HashSet<Device> devices = new HashSet<Device>();

            foreach (var device in FiresecManager.Configuration.Devices)
            {
                if (device.Driver.IsZoneDevice)
                {
                    if (device.ZoneNo == SelectedZone.Zone.No)
                    {
                        device.AllParents.ForEach(x => { devices.Add(x); });
                        devices.Add(device);
                    }
                }

                if (device.Driver.IsZoneLogicDevice)
                {
                    if (device.ZoneLogic != null)
                    {
                        foreach (var clause in device.ZoneLogic.Clauses)
                        {
                            if (clause.Zones.Contains(SelectedZone.Zone.No))
                            {
                                device.AllParents.ForEach(x => { devices.Add(x); });
                                devices.Add(device);
                            }
                        }
                    }
                }
            }

            Devices = new ObservableCollection<DeviceViewModel>();
            foreach (var device in devices)
            {
                DeviceViewModel deviceViewModel = new DeviceViewModel();
                deviceViewModel.Initialize(device, Devices);
                deviceViewModel.IsExpanded = true;
                Devices.Add(deviceViewModel);
            }

            foreach (var device in Devices)
            {
                if (device.Device.Parent != null)
                {
                    var parent = Devices.FirstOrDefault(x => x.Device.Id == device.Device.Parent.Id);
                    device.Parent = parent;
                    parent.Children.Add(device);
                }
            }
        }
    }
}
