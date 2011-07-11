using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecClient;
using System.Collections.ObjectModel;
using Infrastructure;
using FiresecClient.Models;

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

                HashSet<Device> devices = new HashSet<Device>();
                HashSet<Device> availableDevices = new HashSet<Device>();

                foreach (var device in FiresecManager.Configuration.Devices)
                {
                    if (device.Driver.IsZoneDevice)
                    {
                        if (string.IsNullOrEmpty(device.ZoneNo))
                        {
                            device.AllParents.ForEach(x => { availableDevices.Add(x); });
                            availableDevices.Add(device);
                        }

                        if (device.ZoneNo == zoneNo)
                        {
                            device.AllParents.ForEach(x => { devices.Add(x); });
                            devices.Add(device);
                        }
                    }

                    if (device.Driver.IsZoneLogicDevice)
                    {
                        if ((device.ZoneLogic != null) && (device.ZoneLogic.clause != null))
                        {
                            foreach (var clause in device.ZoneLogic.clause)
                            {
                                if ((clause.zone != null) && (clause.zone.Contains(zoneNo)))
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

            AvailableDevices = new ObservableCollection<DeviceViewModel>();
            foreach (var device in availableDevices)
            {
                DeviceViewModel deviceViewModel = new DeviceViewModel();
                deviceViewModel.Initialize(device, AvailableDevices);
                deviceViewModel.IsExpanded = true;
                AvailableDevices.Add(deviceViewModel);
            }

            foreach (var device in AvailableDevices)
            {
                if (device.Device.Parent != null)
                {
                    var parent = AvailableDevices.FirstOrDefault(x => x.Device.Id == device.Device.Parent.Id);
                    device.Parent = parent;
                    parent.Children.Add(device);
                }
            }

            SelectedDevice = Devices[4];
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

        public bool CanAdd(object obj)
        {
            return ((SelectedAvailableDevice != null) && (SelectedAvailableDevice.Device.Driver.IsZoneDevice));
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            if (CanAdd(null))
            {
                SelectedAvailableDevice.Device.ZoneNo = _zoneNo;
                Initialize(_zoneNo);
            }
        }

        public bool CanRemove(object obj)
        {
            return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.IsZoneDevice));
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            if (CanRemove(null))
            {
                SelectedDevice.Device.ZoneNo = null;
                Initialize(_zoneNo);
            }
        }

        public bool CanShowZoneLogic(object obj)
        {
            return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.IsZoneLogicDevice));
        }

        public RelayCommand ShowZoneLogicCommand { get; private set; }
        void OnShowZoneLogic()
        {
            if (CanShowZoneLogic(null))
            {
                ZoneLogicViewModel zoneLogicViewModel = new ZoneLogicViewModel();
                zoneLogicViewModel.Initialize(SelectedDevice.Device.ZoneLogic);
                bool result = ServiceFactory.UserDialogs.ShowModalWindow(zoneLogicViewModel);
                if (result)
                {
                    SelectedDevice.Device.ZoneLogic = zoneLogicViewModel.Save();
                }
            }
        }
    }
}
