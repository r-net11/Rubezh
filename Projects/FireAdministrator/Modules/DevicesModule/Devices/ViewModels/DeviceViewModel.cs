using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevicesModule.DeviceProperties;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Controls.MessageBox;

namespace DevicesModule.ViewModels
{
    public class DeviceViewModel : TreeBaseViewModel<DeviceViewModel>
    {
        public Device Device { get; private set; }
        public PropertiesViewModel PropertiesViewModel { get; private set; }

        public DeviceViewModel(Device device, ObservableCollection<DeviceViewModel> sourceDevices)
        {
            Children = new ObservableCollection<DeviceViewModel>();

            AddCommand = new RelayCommand(OnAdd, CanAdd);
            AddManyCommand = new RelayCommand(OnAddMany, CanAdd);
            RemoveCommand = new RelayCommand(OnRemove, CanRemove);
            ShowZoneLogicCommand = new RelayCommand(OnShowZoneLogic);
            ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
            ShowZoneCommand = new RelayCommand(OnShowZone);

            Source = sourceDevices;
            Device = device;
            PropertiesViewModel = new PropertiesViewModel(device);

            AvailvableDrivers = new ObservableCollection<Driver>();
            UpdateDriver();
        }

        public void Update()
        {
            IsExpanded = false;
            IsExpanded = true;
            OnPropertyChanged("HasChildren");
        }

        public Guid UID
        {
            get { return Device.UID; }
        }

        public Driver Driver
        {
            get { return Device.Driver; }
            set
            {
                Device.Driver = value;
                OnPropertyChanged("Device");
                OnPropertyChanged("Device.Driver");
                OnPropertyChanged("PresentationZone");
            }
        }

        public string Address
        {
            get { return Device.PresentationAddress; }
            set
            {
                if (Device.Parent.Children.Where(x => x != Device).Any(x => x.PresentationAddress == value))
                {
                    MessageBoxService.Show("Устройство с таким адресом уже существует");
                }
                else
                {
                    Device.SetAddress(value);
                    if (Driver.IsChildAddressReservedRange)
                    {
                        foreach (var deviceViewModel in Children)
                        {
                            deviceViewModel.OnPropertyChanged("Address");
                        }
                    }
                    ServiceFactory.SaveService.DevicesChanged = true;
                }
                OnPropertyChanged("Address");
            }
        }

        public bool CanEditAddress
        {
            get
            {
                if (Parent != null && Parent.Driver.IsChildAddressReservedRange && Parent.Driver.DriverType != DriverType.MRK_30)
                    return false;
                return Driver.CanEditAddress;
            }
        }

        public string Description
        {
            get { return Device.Description; }
            set
            {
                Device.Description = value;
                OnPropertyChanged("Description");

                ServiceFactory.SaveService.DevicesChanged = true;
            }
        }

        public IEnumerable<Zone> Zones
        {
            get
            {
                return from Zone zone in FiresecManager.DeviceConfiguration.Zones
                       orderby zone.No
                       select zone;
            }
        }

        public Zone Zone
        {
            get { return FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == Device.ZoneNo); }
            set
            {
                Device.ZoneNo = value.No;
                OnPropertyChanged("Zone");

                ServiceFactory.SaveService.DevicesChanged = true;
            }
        }

        public string PresentationZone
        {
            get
            {
                if (Device.Driver.IsZoneDevice)
                {
                    var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == Device.ZoneNo);
                    if (zone != null)
                        return zone.PresentationName;
                }

                if (Device.Driver.IsZoneLogicDevice && Device.ZoneLogic != null)
                    return Device.ZoneLogic.ToString();
                if (Device.Driver.IsIndicatorDevice && Device.IndicatorLogic != null)
                    return Device.IndicatorLogic.ToString();
                if ((Device.Driver.DriverType == DriverType.Direction) && (Device.PDUGroupLogic != null))
                    return Device.PDUGroupLogic.ToString();
                return "";
            }
        }

        public string ConnectedTo
        {
            get { return Device.ConnectedTo; }
        }

        public RelayCommand ShowZoneLogicCommand { get; private set; }
        void OnShowZoneLogic()
        {
            var zoneLogicViewModel = new ZoneLogicViewModel(Device);
            if (ServiceFactory.UserDialogs.ShowModalWindow(zoneLogicViewModel))
                ServiceFactory.SaveService.DevicesChanged = true;
            OnPropertyChanged("PresentationZone");
        }

        void OnShowIndicatorLogic()
        {
            var indicatorDetailsViewModel = new IndicatorDetailsViewModel(Device);
            if (ServiceFactory.UserDialogs.ShowModalWindow(indicatorDetailsViewModel))
                ServiceFactory.SaveService.DevicesChanged = true;
            OnPropertyChanged("PresentationZone");
        }

        public bool CanAdd()
        {
            return (Driver.CanAddChildren && Driver.AutoChild == Guid.Empty);
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            if (ServiceFactory.UserDialogs.ShowModalWindow(new NewDeviceViewModel(this)))
            {
                ServiceFactory.SaveService.DevicesChanged = true;
                DevicesViewModel.UpdateGuardVisibility();
            }
        }

        public RelayCommand AddManyCommand { get; private set; }
        void OnAddMany()
        {
            if (ServiceFactory.UserDialogs.ShowModalWindow(new NewDeviceRangeViewModel(this)))
            {
                ServiceFactory.SaveService.DevicesChanged = true;
                DevicesViewModel.UpdateGuardVisibility();
            }
        }

        bool CanRemove()
        {
            return !(Driver.IsAutoCreate || Parent == null || Parent.Driver.AutoChild == Driver.UID);
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            Parent.IsExpanded = false;
            Parent.Device.Children.Remove(Device);
            Parent.Children.Remove(this);
            Parent.Update();
            Parent.IsExpanded = true;
            Parent = null;

            FiresecManager.DeviceConfiguration.Update();
            ServiceFactory.SaveService.DevicesChanged = true;
            DevicesViewModel.UpdateGuardVisibility();
            FiresecManager.InvalidateConfiguration();
        }

        bool CanShowProperties()
        {
            switch (Device.Driver.DriverType)
            {
                case DriverType.Indicator:
                case DriverType.Valve:
                case DriverType.Pump:
                case DriverType.JokeyPump:
                case DriverType.Compressor:
                case DriverType.CompensationPump:
                case DriverType.Direction:
                case DriverType.UOO_TL:
                case DriverType.MPT:
                    return true;
            }
            return false;
        }

        public RelayCommand ShowPropertiesCommand { get; private set; }
        void OnShowProperties()
        {
            switch (Device.Driver.DriverType)
            {
                case DriverType.Indicator:
                    OnShowIndicatorLogic();
                    break;

                case DriverType.Valve:
                    if (ServiceFactory.UserDialogs.ShowModalWindow(new ValveDetailsViewModel(Device)))
                        ServiceFactory.SaveService.DevicesChanged = true;
                    break;

                case DriverType.Pump:
                case DriverType.JokeyPump:
                case DriverType.Compressor:
                case DriverType.CompensationPump:
                    if (ServiceFactory.UserDialogs.ShowModalWindow(new PumpDetailsViewModel(Device)))
                        ServiceFactory.SaveService.DevicesChanged = true;
                    break;

                case DriverType.Direction:
                    if (ServiceFactory.UserDialogs.ShowModalWindow(new GroupDetailsViewModel(Device)))
                        ServiceFactory.SaveService.DevicesChanged = true;
                    break;

                case DriverType.UOO_TL:
                    if (ServiceFactory.UserDialogs.ShowModalWindow(new TelephoneLineDetailsViewModel(Device)))
                        ServiceFactory.SaveService.DevicesChanged = true;
                    break;

                case DriverType.MPT:
                    if (ServiceFactory.UserDialogs.ShowModalWindow(new MptDetailsViewModel(Device)))
                        ServiceFactory.SaveService.DevicesChanged = true;
                    break;
            }
            OnPropertyChanged("PresentationZone");
        }

        public RelayCommand ShowZoneCommand { get; private set; }
        void OnShowZone()
        {
            if (Device.ZoneNo.HasValue)
                ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(Device.ZoneNo.Value);
        }

        public ObservableCollection<Driver> AvailvableDrivers { get; private set; }

        void UpdateDriver()
        {
            AvailvableDrivers.Clear();
            if (CanChangeDriver)
            {
                foreach (var driverUID in Device.Parent.Driver.AvaliableChildren)
                {
                    var driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == driverUID);
                    if (CanDriverBeChanged(driver))
                    {
                        AvailvableDrivers.Add(driver);
                    }
                }
            }
        }

        public bool CanDriverBeChanged(Driver driver)
        {
            if (driver == null)
                return false;
            if (driver.IsAutoCreate)
                return false;
            if (Parent != null && Parent.Driver.IsChildAddressReservedRange)
                return false;
            return (driver.Category == DeviceCategoryType.Sensor) || (driver.Category == DeviceCategoryType.Effector);
        }

        public bool CanChangeDriver
        {
            get
            {
                return CanDriverBeChanged(Device.Driver);
            }
        }

        public RelayCommand CopyCommand { get { return DevicesViewModel.Current.CopyCommand; } }
        public RelayCommand CutCommand { get { return DevicesViewModel.Current.CutCommand; } }
        public RelayCommand PasteCommand { get { return DevicesViewModel.Current.PasteCommand; } }
        public RelayCommand PasteAsCommand { get { return DevicesViewModel.Current.PasteAsCommand; } }
    }
}