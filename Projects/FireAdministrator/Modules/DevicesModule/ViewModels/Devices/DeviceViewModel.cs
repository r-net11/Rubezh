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
        public Device Device { get; private set; }

        public PropertiesViewModel PropertiesViewModel { get; private set; }

        public void Update()
        {
            IsExpanded = false;
            IsExpanded = true;
            OnPropertyChanged("HasChildren");
        }

        public string Id
        {
            get { return Device.Id; }
        }

        public Driver Driver
        {
            get { return Device.Driver; }
        }

        public string Address
        {
            get { return Device.Driver.HasAddress ? Device.Address : ""; }
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

        public IEnumerable<Zone> Zones
        {
            get
            {
                return from Zone zone in FiresecManager.Configuration.Zones
                       orderby Convert.ToInt32(zone.No)
                       select zone;
            }
        }

        public Zone Zone
        {
            get
            {
                return FiresecManager.Configuration.Zones.FirstOrDefault(x => x.No == Device.ZoneNo);
            }
            set
            {
                Device.ZoneNo = value.No;
                OnPropertyChanged("Zone");
            }
        }

        public string ConnectedTo
        {
            get { return Device.ConnectedTo; }
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
                Parent.Device.Children.Remove(Device);
                Parent.Children.Remove(this);
                Parent.Update();
                Parent.IsExpanded = true;

                FiresecManager.Configuration.Update();
            }
        }
    }
}
