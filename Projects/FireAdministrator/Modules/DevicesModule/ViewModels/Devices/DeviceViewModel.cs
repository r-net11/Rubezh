using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure.Common;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using DevicesModule.Views;
using Infrastructure;
using DevicesModule.DeviceProperties;
using FiresecClient.Models;

namespace DevicesModule.ViewModels
{
    public class DeviceViewModel : TreeBaseViewModel<DeviceViewModel>
    {
        public Device Device { get; set; }

        public DeviceViewModel()
        {
            Children = new ObservableCollection<DeviceViewModel>();
            AddCommand = new RelayCommand(OnAdd);
            AddManyCommand = new RelayCommand(OnAddMany);
            RemoveCommand = new RelayCommand(OnRemove);
            ShowZoneLogicCommand = new RelayCommand(OnShowZoneLogic);
            ShowIndicatorLogicCommand = new RelayCommand(OnShowIndicatorLogic);
        }

        public void Initialize(Device device, ObservableCollection<DeviceViewModel> sourceDevices)
        {
            Source = sourceDevices;
            Device = device;
            PropertiesViewModel = new DeviceProperties.PropertiesViewModel(device);
            Address = device.Address;
            Description = device.Description;
        }

        public PropertiesViewModel PropertiesViewModel { get; private set; }

        public void Update()
        {
            IsExpanded = false;
            IsExpanded = true;
            OnPropertyChanged("HasChildren");
        }

        public string DriverId
        {
            get { return Device.DriverId; }
        }

        public bool IsZoneDevice
        {
            get { return Device.Driver.IsZoneDevice(); }
        }

        public bool IsIndicatorDevice
        {
            get { return Device.Driver.IsIndicatorDevice(); }
        }

        public bool IsZoneLogicDevice
        {
            get { return Device.Driver.IsZoneLogicDevice(); }
        }

        public bool CanAddChildren
        {
            get { return Device.Driver.CanAddChildren(); }
        }

        public string ShortDriverName
        {
            get { return Device.Driver.shortName; }
        }

        public string DriverName
        {
            get { return Device.Driver.name; }
        }

        public bool HasAddress
        {
            get { return (!string.IsNullOrEmpty(Address)); }
        }

        public bool CanEditAddress
        {
            get { return Device.Driver.CanEditAddress(); }
        }

        public bool HasImage
        {
            get { return Device.Driver.HasImage(); }
        }

        public string ImageSource
        {
            get { return Device.Driver.ImageSource(); }
        }

        public string Address
        {
            get
            {
                if (Device.Driver.CanEditAddress())
                {
                    return Device.Address;
                }
                return "";
            }
            set
            {
                Device.Address = value;
                OnPropertyChanged("Address");
            }
        }

        public string Description
        {
            get { return Device.Description; }
            set
            {
                Device.Description = value;
                OnPropertyChanged("Description");
            }
        }

        public List<ZoneViewModel> Zones
        {
            get
            {
                List<ZoneViewModel> zones = new List<ZoneViewModel>();
                FiresecManager.Configuration.Zones.ForEach(x => { zones.Add(new ZoneViewModel(x)); });
                return zones;
            }
        }

        public ZoneViewModel Zone
        {
            get
            {
                Zone zone = FiresecManager.Configuration.Zones.FirstOrDefault(x => x.No == Device.ZoneNo);
                if (zone != null)
                {
                    ZoneViewModel zoneViewModel = new ZoneViewModel(zone);
                    return zoneViewModel;
                }
                return null;
            }
            set
            {
                Device.ZoneNo = value.No;
                OnPropertyChanged("Zone");
            }
        }

        public string ConnectedTo
        {
            get
            {
                if (Parent == null)
                    return null;
                else
                {
                    string parentPart = Parent.Device.Driver.shortName;
                    if (Parent.Device.Driver.ar_no_addr != "1")
                        parentPart += " - " + Parent.Address;

                    if (Parent.ConnectedTo == null)
                        return parentPart;

                    if (Parent.Parent.ConnectedTo == null)
                        return parentPart;

                    return parentPart + @"\" + Parent.ConnectedTo;
                }
            }
        }

        public RelayCommand ShowZoneLogicCommand { get; private set; }
        void OnShowZoneLogic()
        {
            ZoneLogicViewModel zoneLogicViewModel = new ZoneLogicViewModel();
            zoneLogicViewModel.Initialize(Device.ZoneLogic);
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(zoneLogicViewModel);
            if (result)
            {
                Device.ZoneLogic = zoneLogicViewModel.Save();
            }
        }

        public RelayCommand ShowIndicatorLogicCommand { get; private set; }
        void OnShowIndicatorLogic()
        {
            IndicatorDetailsViewModel indicatorDetailsViewModel = new IndicatorDetailsViewModel();
            indicatorDetailsViewModel.Initialize(Device);
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(indicatorDetailsViewModel);
            if (result)
            {
                ;
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            NewDeviceViewModel newDeviceViewModel = new NewDeviceViewModel(this);
            ServiceFactory.UserDialogs.ShowModalWindow(newDeviceViewModel);
        }

        public RelayCommand AddManyCommand { get; private set; }
        void OnAddMany()
        {
            NewDeviceViewModel newDeviceViewModel = new NewDeviceViewModel(this);
            ServiceFactory.UserDialogs.ShowModalWindow(newDeviceViewModel);
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            if (Parent != null)
            {
                Parent.IsExpanded = false;
                Parent.Device.Children.Remove(Parent.Device.Children.FirstOrDefault(x => x.Id == Device.Id));
                Parent.Children.Remove(this);
                Parent.Update();
                Parent.IsExpanded = true;
            }
        }
    }
}
