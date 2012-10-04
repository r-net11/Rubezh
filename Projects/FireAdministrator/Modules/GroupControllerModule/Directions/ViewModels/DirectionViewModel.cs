using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using Infrastructure.Common.Windows;
using Infrastructure;
using System.Windows.Documents;
using System.Collections.Generic;
using System;

namespace GKModule.ViewModels
{
    public class DirectionViewModel : BaseViewModel
    {
        public XDirection Direction { get; set; }

        public DirectionViewModel(XDirection direction)
        {
            Direction = direction;
            Zones = new ObservableCollection<ZoneViewModel>();
            Devices = new ObservableCollection<DeviceViewModel>();
            InitializeDirectionZones();
            InitializeDirectionDevices();
        }

        void InitializeDirectionZones()
        {
            Zones.Clear();
            foreach (var zone in Direction.Zones)
            {
                var zoneViewModel = new ZoneViewModel(zone);
                Zones.Add(zoneViewModel);
            }
        }

        void InitializeDirectionDevices()
        {
            Devices.Clear();
            foreach (var directionDevice in Direction.DirectionDevices)
            {
                var deviceViewModel = new DeviceViewModel(directionDevice.Device, null);
                Devices.Add(deviceViewModel);
            }
        }

        public void Update()
        {
            OnPropertyChanged("Direction");
        }

        public ObservableCollection<ZoneViewModel> Zones { get; private set; }

        ZoneViewModel _selectedZone;
        public ZoneViewModel SelectedZone
        {
            get { return _selectedZone; }
            set
            {
                _selectedZone = value;
                OnPropertyChanged("SelectedZone");
            }
        }

        public ObservableCollection<DeviceViewModel> Devices { get; private set; }

        DeviceViewModel _selectedDevice;
        public DeviceViewModel SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }

        public void ChangeZones()
        {
            var zonesSelectationViewModel = new ZonesSelectationViewModel(Direction.ZoneUIDs);
            if (DialogService.ShowModalWindow(zonesSelectationViewModel))
            {
                XManager.ChangeDirectionZones(Direction, zonesSelectationViewModel.Zones);
                InitializeDirectionZones();
                ServiceFactory.SaveService.XDevicesChanged = true;
            }
        }

        public void ChangeDevices()
        {
            var deviceUIDs = new List<Guid>();
            foreach (var directionDevice in Direction.DirectionDevices)
            {
                deviceUIDs.Add(directionDevice.DeviceUID);
            }
            var devicesSelectationViewModel = new DevicesSelectationViewModel(deviceUIDs);
            if (DialogService.ShowModalWindow(devicesSelectationViewModel))
            {
                Direction.DirectionDevices = new List<DirectionDevice>();
                foreach (var deviceUID in devicesSelectationViewModel.DevicesList)
                {
                    var device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
                    var directionDevice = new DirectionDevice()
                    {
                        Device = device,
                        DeviceUID = deviceUID,
                        StateType = XStateType.TurningOn
                    };
                    Direction.DirectionDevices.Add(directionDevice);
                }
                InitializeDirectionDevices();
                ServiceFactory.SaveService.XDevicesChanged = true;
            }
        }
    }
}