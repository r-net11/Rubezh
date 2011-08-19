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
        public DeviceViewModel()
        {
            Children = new ObservableCollection<DeviceViewModel>();
            AddCommand = new RelayCommand(OnAdd, CanAdd);
            AddManyCommand = new RelayCommand(OnAddMany, CanAdd);
            RemoveCommand = new RelayCommand(OnRemove, CanRemove);
            ShowZoneLogicCommand = new RelayCommand(OnShowZoneLogic);
            ShowIndicatorLogicCommand = new RelayCommand(OnShowIndicatorLogic);
            ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
        }

        public void Initialize(Device device, ObservableCollection<DeviceViewModel> sourceDevices)
        {
            Source = sourceDevices;
            Device = device;
            PropertiesViewModel = new DeviceProperties.PropertiesViewModel(device);
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
            get { return Device.Driver.HasAddress ? Device.PresentationAddress : ""; }
            set
            {
                Device.SetAddress(value);
                OnPropertyChanged("Address");
                DevicesModule.HasChanges = true;
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
                       orderby int.Parse(zone.No)
                       select zone;
            }
        }

        public Zone Zone
        {
            get
            {
                return FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == Device.ZoneNo);
            }
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
                    return "Зона";

                if (Device.Driver.IsZoneLogicDevice)
                    return "Логика Зон";

                if (Device.Driver.IsIndicatorDevice)
                    return "Индикатор";

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
            var zoneLogicViewModel = new ZoneLogicViewModel();
            zoneLogicViewModel.Initialize(Device);
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(zoneLogicViewModel);
            if (result)
            {
                DevicesModule.HasChanges = true;
            }
        }

        public RelayCommand ShowIndicatorLogicCommand { get; private set; }
        void OnShowIndicatorLogic()
        {
            var indicatorDetailsViewModel = new IndicatorDetailsViewModel();
            indicatorDetailsViewModel.Initialize(Device);
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(indicatorDetailsViewModel);
            if (result)
            {
                DevicesModule.HasChanges = true;
            }
        }

        public bool CanAdd(object obj)
        {
            return Driver.CanAddChildren;
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            if (CanAdd(null))
            {
                var newDeviceViewModel = new NewDeviceViewModel(this);
                bool result = ServiceFactory.UserDialogs.ShowModalWindow(newDeviceViewModel);
                if (result)
                {
                    DevicesModule.HasChanges = true;
                }
            }
        }

        public RelayCommand AddManyCommand { get; private set; }
        void OnAddMany()
        {
            if (CanAdd(null))
            {
                var newDeviceRangeViewModel = new NewDeviceRangeViewModel(this);
                bool result = ServiceFactory.UserDialogs.ShowModalWindow(newDeviceRangeViewModel);
                if (result)
                {
                    DevicesModule.HasChanges = true;
                }
            }
        }

        bool CanRemove(object obj)
        {
            if (Parent == null)
                return false;

            if (Driver.IsAutoCreate)
                return false;

            return true;
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            if (CanRemove(null))
            {
                Parent.IsExpanded = false;
                Parent.Device.Children.Remove(Device);
                Parent.Children.Remove(this);
                Parent.Update();
                Parent.IsExpanded = true;

                FiresecManager.DeviceConfiguration.Update();
                DevicesModule.HasChanges = true;
            }
        }

        bool CanShowProperties(object obj)
        {
            switch (Device.Driver.DriverName)
            {
                case "Индикатор":
                case "Задвижка":
                case "Насос":
                case "Жокей-насос":
                case "Компрессор":
                case "Насос компенсации утечек":
                case "Группа":
                    return true;
            }
            return false;
        }

        public RelayCommand ShowPropertiesCommand { get; private set; }
        void OnShowProperties()
        {
            bool result = false;

            switch (Device.Driver.DriverName)
            {
                case "Индикатор":
                    OnShowIndicatorLogic();
                    break;

                case "Задвижка":
                    var valveDetailsViewModel = new ValveDetailsViewModel(Device);
                    result = ServiceFactory.UserDialogs.ShowModalWindow(valveDetailsViewModel);
                    break;

                case "Насос":
                case "Жокей-насос":
                case "Компрессор":
                case "Насос компенсации утечек":
                    var pumpDetailsViewModel = new PumpDetailsViewModel(Device);
                    result = ServiceFactory.UserDialogs.ShowModalWindow(pumpDetailsViewModel);
                    break;

                case "Группа":
                    var groupDetailsViewModel = new GroupDetailsViewModel(Device);
                    result = ServiceFactory.UserDialogs.ShowModalWindow(groupDetailsViewModel);
                    break;
            }

            if (result)
            {
                DevicesModule.HasChanges = true;
            }
        }
    }
}