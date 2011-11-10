using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevicesModule.DeviceProperties;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

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
            ShowIndicatorLogicCommand = new RelayCommand(OnShowIndicatorLogic);
            ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);

            Source = sourceDevices;
            Device = device;
            PropertiesViewModel = new DeviceProperties.PropertiesViewModel(device);
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
        }

        public string Address
        {
            get { return Device.PresentationAddress; }
            set
            {
                if (Device.Parent.Children.Where(x => x != Device).Any(x => x.PresentationAddress == value))
                {
                    DialogBox.DialogBox.Show("Устройство с таким адресом уже существует");
                    OnPropertyChanged("Address");
                }
                else
                {
                    Device.SetAddress(value);
                    OnPropertyChanged("Address");

                    DevicesModule.HasChanges = true;
                }
            }
        }

        public string Description
        {
            get { return Device.Description; }
            set
            {
                Device.Description = value;
                OnPropertyChanged("Description");

                DevicesModule.HasChanges = true;
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

                DevicesModule.HasChanges = true;
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
            DevicesModule.HasChanges = ServiceFactory.UserDialogs.ShowModalWindow(zoneLogicViewModel);
        }

        public RelayCommand ShowIndicatorLogicCommand { get; private set; }
        void OnShowIndicatorLogic()
        {
            var indicatorDetailsViewModel = new IndicatorDetailsViewModel();
            indicatorDetailsViewModel.Initialize(Device);

            DevicesModule.HasChanges = ServiceFactory.UserDialogs.ShowModalWindow(indicatorDetailsViewModel);
        }

        public bool CanAdd()
        {
            return (Driver.CanAddChildren && Driver.AutoChild == Guid.Empty);
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            if (ServiceFactory.UserDialogs.ShowModalWindow(new NewDeviceViewModel(this)))
                DevicesModule.HasChanges = true;
        }

        public RelayCommand AddManyCommand { get; private set; }
        void OnAddMany()
        {
            if (ServiceFactory.UserDialogs.ShowModalWindow(new NewDeviceRangeViewModel(this)))
                DevicesModule.HasChanges = true;
        }

        bool CanRemove()
        {
            return !(Parent == null || Driver.IsAutoCreate || Parent.Driver.AutoChild == Driver.UID);
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            Parent.IsExpanded = false;
            Parent.Device.Children.Remove(Device);
            Parent.Children.Remove(this);
            Parent.Update();
            Parent.IsExpanded = true;

            FiresecManager.DeviceConfiguration.Update();

            DevicesModule.HasChanges = true;
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
                case DriverType.Group:
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
                    DevicesModule.HasChanges = ServiceFactory.UserDialogs.ShowModalWindow(new ValveDetailsViewModel(Device));
                    break;

                case DriverType.Pump:
                case DriverType.JokeyPump:
                case DriverType.Compressor:
                case DriverType.CompensationPump:
                    DevicesModule.HasChanges = ServiceFactory.UserDialogs.ShowModalWindow(new PumpDetailsViewModel(Device));
                    break;

                case DriverType.Group:
                    DevicesModule.HasChanges = ServiceFactory.UserDialogs.ShowModalWindow(new GroupDetailsViewModel(Device));
                    break;
            }
        }
    }
}