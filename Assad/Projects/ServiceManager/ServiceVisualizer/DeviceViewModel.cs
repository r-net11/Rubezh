using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Common;
using ClientApi;
using System.Collections.ObjectModel;

namespace ServiceVisualizer
{
    public class DeviceViewModel : BaseViewModel
    {
        public DeviceViewModel()
        {
            Children = new List<DeviceViewModel>();
            AddCommand = new RelayCommand(OnAddCommand);
        }

        public RelayCommand AddCommand { get; private set; }

        void OnAddCommand(object obj)
        {
            NewDeviceViewModel newDeviceViewModel = new NewDeviceViewModel();
            newDeviceViewModel.Init(this);
            NewDeviceView newDeviceView = new NewDeviceView();
            newDeviceView.DataContext = newDeviceViewModel;
            newDeviceView.ShowDialog();
        }

        Device device;
        Firesec.Metadata.drvType driver;

        public string DriverId
        {
            get
            {
                if (device != null)
                    return device.DriverId;
                return null;
            }
        }

        public void SetDevice(Device device)
        {
            this.device = device;
            driver = ServiceClient.Configuration.Metadata.drv.FirstOrDefault(x => x.id == device.DriverId);

            DriverName = device.DriverName;
            ShortDriverName = device.ShortDriverName;
            Address = device.PresentationAddress;
            Description = device.Description;
            ImageSource = @"C:\Program Files\Firesec\Icons\" + device.ImageName + ".ico";
            State = device.State;
            States = new ObservableCollection<string>();
            foreach (string state in device.States)
            {
                States.Add(state);
            }
            Parameters = new ObservableCollection<Parameter>();
            foreach (Parameter parameter in device.Parameters)
            {
                Parameters.Add(parameter);
            }
        }

        public void SetZone()
        {
            if (device._Zone != null)
            {
                Zone = ViewModel.Current.ZoneViewModels.FirstOrDefault(x => x.ZoneNo == device._Zone.No);
            }
        }

        public ObservableCollection<ZoneViewModel> Zones
        {
            get { return ViewModel.Current.ZoneViewModels; }
        }

        public bool IsZoneDevice
        {
            get
            {
                if ((driver.minZoneCardinality == "0") && (driver.maxZoneCardinality == "0"))
                {
                    return false;
                }
                return true;
            }
        }

        public bool IsZoneLogicDevice
        {
            get
            {
                if ((driver.options != null) && (driver.options.Contains("ExtendedZoneLogic")))
                {
                    return true;
                }
                return false;
            }
        }

        public string AvailableChildren
        {
            get
            {
                string allDrivers = "";
                FiresecMetadata.DriverItem driverItem = ViewModel.Current.treeBuilder.Drivers.FirstOrDefault(x => x.DriverId == device.DriverId);
                foreach (FiresecMetadata.DriverItem childDriverItem in driverItem.Children)
                {
                    Firesec.Metadata.drvType currentDriver = ServiceClient.Configuration.Metadata.drv.FirstOrDefault(x => x.id == childDriverItem.DriverId);
                    allDrivers += currentDriver.name + "\n";
                }
                return allDrivers;
            }
        }

        public bool CanAddChildren
        {
            get
            {
                FiresecMetadata.DriverItem driverItem = ViewModel.Current.treeBuilder.Drivers.FirstOrDefault(x => x.DriverId == device.DriverId);
                if (driverItem == null)
                    return false;
                if (driverItem.Children.Count > 0)
                    return true;
                return false;
            }
        }

        string shortDriverName;
        public string ShortDriverName
        {
            get { return shortDriverName; }
            set
            {
                shortDriverName = value;
                OnPropertyChanged("ShortDriverName");
            }
        }

        string driverName;
        public string DriverName
        {
            get { return driverName; }
            set
            {
                driverName = value;
                OnPropertyChanged("DriverName");
            }
        }

        string address;
        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                OnPropertyChanged("Address");
            }
        }

        ZoneViewModel zone;
        public ZoneViewModel Zone
        {
            get { return zone; }
            set
            {
                zone = value;
                OnPropertyChanged("Zone");
            }
        }

        string description;
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }

        string state;
        public string State
        {
            get { return state; }
            set
            {
                state = value;
                OnPropertyChanged("State");
            }
        }

        ObservableCollection<string> states;
        public ObservableCollection<string> States
        {
            get { return states; }
            set
            {
                states = value;
                OnPropertyChanged("States");
            }
        }

        ObservableCollection<Parameter> parameters;
        public ObservableCollection<Parameter> Parameters
        {
            get { return parameters; }
            set
            {
                parameters = value;
                OnPropertyChanged("Parameters");
            }
        }

        string imageSource;
        public string ImageSource
        {
            get { return imageSource; }
            set
            {
                imageSource = value;
                OnPropertyChanged("ImageSource");
            }
        }

        public DeviceViewModel Parent { get; set; }

        List<DeviceViewModel> children;
        public List<DeviceViewModel> Children
        {
            get { return children; }
            set
            {
                children = value;
                OnPropertyChanged("Children");
            }
        }

        bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
                ViewModel.Current.SelectedDeviceViewModel = this;
            }
        }

        bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }
    }
}
