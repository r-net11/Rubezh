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
            Zones = new ObservableCollection<DirectionZoneViewModel>();
            Devices = new ObservableCollection<DirectionDeviceViewModel>();
            InitializeDirectionZones();
            InitializeDirectionDevices();
        }

        void InitializeDirectionZones()
        {
            Zones.Clear();
            foreach (var directionZone in Direction.DirectionZones)
            {
                var directionZoneViewModel = new DirectionZoneViewModel(directionZone);
                Zones.Add(directionZoneViewModel);
            }
        }

        void InitializeDirectionDevices()
        {
            Devices.Clear();
            foreach (var directionDevice in Direction.DirectionDevices)
            {
                var directionDeviceViewModel = new DirectionDeviceViewModel(directionDevice);
                Devices.Add(directionDeviceViewModel);
            }
        }

        public void Update()
        {
            OnPropertyChanged("Direction");
        }

        public ObservableCollection<DirectionZoneViewModel> Zones { get; private set; }

        DirectionZoneViewModel _selectedZone;
        public DirectionZoneViewModel SelectedZone
        {
            get { return _selectedZone; }
            set
            {
                _selectedZone = value;
                OnPropertyChanged("SelectedZone");
            }
        }

        public ObservableCollection<DirectionDeviceViewModel> Devices { get; private set; }

        DirectionDeviceViewModel _selectedDevice;
        public DirectionDeviceViewModel SelectedDevice
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
            var zonesSelectationViewModel = new ZonesSelectationViewModel(Direction.InputZones);
            if (DialogService.ShowModalWindow(zonesSelectationViewModel))
            {
                XManager.ChangeDirectionZones(Direction, zonesSelectationViewModel.Zones);
                InitializeDirectionZones();
                ServiceFactory.SaveService.XDevicesChanged = true;
            }
        }

        public void ChangeDevices()
        {
            var devicesSelectationViewModel = new DevicesSelectationViewModel(Direction.InputDevices);
            if (DialogService.ShowModalWindow(devicesSelectationViewModel))
            {
                XManager.ChangeDirectionDevices(Direction, devicesSelectationViewModel.DevicesList);
                InitializeDirectionDevices();
                ServiceFactory.SaveService.XDevicesChanged = true;
            }
        }
    }
}