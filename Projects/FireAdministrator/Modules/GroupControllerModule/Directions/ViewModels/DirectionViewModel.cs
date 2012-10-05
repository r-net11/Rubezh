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
            foreach (var device in Direction.Devices)
            {
                var deviceViewModel = new DeviceViewModel(device, null);
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
            var zonesSelectationViewModel = new ZonesSelectationViewModel(Direction.Zones);
            if (DialogService.ShowModalWindow(zonesSelectationViewModel))
            {
                XManager.ChangeDirectionZones(Direction, zonesSelectationViewModel.Zones);
                InitializeDirectionZones();
                ServiceFactory.SaveService.XDevicesChanged = true;
            }
        }

        public void ChangeDevices()
        {
            var devicesSelectationViewModel = new DevicesSelectationViewModel(Direction.Devices);
            if (DialogService.ShowModalWindow(devicesSelectationViewModel))
            {
                XManager.ChangeDirectionDevices(Direction, devicesSelectationViewModel.DevicesList);
                InitializeDirectionDevices();
                ServiceFactory.SaveService.XDevicesChanged = true;
            }
        }
    }
}