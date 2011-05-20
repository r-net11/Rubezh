using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecClient;
using System.Collections.ObjectModel;
using Infrastructure;

namespace DevicesModule.ViewModels
{
    public class ZoneDevicesViewModel : BaseViewModel
    {
        public ZoneDevicesViewModel()
        {
            AddCommand = new RelayCommand(OnAdd, CanAdd);
            RemoveCommand = new RelayCommand(OnRemove, CanRemove);
            ShowZoneLogicCommand = new RelayCommand(OnShowZoneLogic, CanShowZoneLogic);
        }

        string _zoneNo;

        public void Initialize(string zoneNo)
        {
            _zoneNo = zoneNo;

            List<Device> devices = new List<Device>();
            List<Device> availableDevices = new List<Device>();

            foreach (Device device in FiresecManager.CurrentConfiguration.AllDevices)
            {
                var driver = FiresecManager.CurrentConfiguration.Metadata.drv.FirstOrDefault(x => x.id == device.DriverId);
                if (!((driver.minZoneCardinality == "0") && (driver.maxZoneCardinality == "0")))
                {
                    if (string.IsNullOrEmpty(device.ZoneNo))
                    {
                        List<Device> allParents = device.AllParents;
                        foreach (var parentDevice in allParents)
                        {
                            if (availableDevices.Any(x => x.Id == parentDevice.Id) == false)
                            {
                                availableDevices.Add(parentDevice);
                            }
                        }
                        availableDevices.Add(device);
                    }

                    if (device.ZoneNo == zoneNo)
                    {
                        List<Device> allParents = device.AllParents;
                        foreach (var parentDevice in allParents)
                        {
                            if (devices.Any(x => x.Id == parentDevice.Id) == false)
                            {
                                devices.Add(parentDevice);
                            }
                        }
                        devices.Add(device);
                    }
                }

                if ((driver.options != null) && (driver.options.Contains("ExtendedZoneLogic")))
                {
                    if (device.ZoneLogic != null)
                    {
                        if (device.ZoneLogic.clause != null)
                        {
                            foreach (var clause in device.ZoneLogic.clause)
                            {
                                if (clause.zone != null)
                                {
                                    if (clause.zone.Contains(zoneNo))
                                    {
                                        List<Device> allParents = device.AllParents;
                                        foreach (var parentDevice in allParents)
                                        {
                                            if (devices.Any(x => x.Id == parentDevice.Id) == false)
                                            {
                                                devices.Add(parentDevice);
                                            }
                                        }
                                        devices.Add(device);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Devices = new ObservableCollection<DeviceViewModel>();
            foreach (Device device in devices)
            {
                DeviceViewModel deviceViewModel = new DeviceViewModel();
                deviceViewModel.Initialize(device, Devices);
                Devices.Add(deviceViewModel);
            }

            foreach (var device in Devices)
            {
                if (device._device.Parent != null)
                {
                    var parent = Devices.FirstOrDefault(x => x._device.Id == device._device.Parent.Id);
                    device.Parent = parent;
                }
            }

            AvailableDevices = new ObservableCollection<DeviceViewModel>();
            foreach (Device device in availableDevices)
            {
                DeviceViewModel deviceViewModel = new DeviceViewModel();
                deviceViewModel.Initialize(device, AvailableDevices);
                AvailableDevices.Add(deviceViewModel);
            }

            foreach (var device in AvailableDevices)
            {
                if (device._device.Parent != null)
                {
                    var parent = AvailableDevices.FirstOrDefault(x => x._device.Id == device._device.Parent.Id);
                    device.Parent = parent;
                }
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

        ObservableCollection<DeviceViewModel> _availableDevices;
        public ObservableCollection<DeviceViewModel> AvailableDevices
        {
            get { return _availableDevices; }
            set
            {
                _availableDevices = value;
                OnPropertyChanged("AvailableDevices");
            }
        }

        DeviceViewModel _selectedAvailableDevice;
        public DeviceViewModel SelectedAvailableDevice
        {
            get { return _selectedAvailableDevice; }
            set
            {
                _selectedAvailableDevice = value;
                OnPropertyChanged("SelectedAvailableDevice");
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            if ((SelectedAvailableDevice != null) && (SelectedAvailableDevice.IsZoneDevice))
            {
                SelectedAvailableDevice._device.ZoneNo = _zoneNo;
                Initialize(_zoneNo);
            }
        }

        public bool CanAdd(object obj)
        {
            return ((SelectedAvailableDevice != null) && (SelectedAvailableDevice.IsZoneDevice));
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            if ((SelectedDevice != null) && (SelectedDevice.IsZoneDevice))
            {
                SelectedDevice._device.ZoneNo = null;
                Initialize(_zoneNo);
            }
        }

        public bool CanRemove(object obj)
        {
            return ((SelectedDevice != null) && (SelectedDevice.IsZoneDevice));
        }

        public RelayCommand ShowZoneLogicCommand { get; private set; }
        void OnShowZoneLogic()
        {
            if ((SelectedDevice != null) && (SelectedDevice.IsZoneLogicDevice))
            {
                ZoneLogicViewModel zoneLogicViewModel = new ZoneLogicViewModel();
                zoneLogicViewModel.Initialize(SelectedDevice);
                ServiceFactory.UserDialogs.ShowModalWindow(zoneLogicViewModel);
            }
        }

        public bool CanShowZoneLogic(object obj)
        {
            return ((SelectedDevice != null) && (SelectedDevice.IsZoneLogicDevice));
        }
    }
}
