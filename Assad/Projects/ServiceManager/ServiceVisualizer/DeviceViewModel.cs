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
        }

        Device device;

        public void SetDevice(Device device)
        {
            this.device = device;
            DriverName = device.DriverName;
            ShortDriverName = device.ShortDriverName;
            Address = device.PresentationAddress;
            if (ServiceClient.Configuration.Zones.Any(x => x.Id == device.Zone))
            {
                Zone _zone = ServiceClient.Configuration.Zones.FirstOrDefault(x => x.Id == device.Zone);
                Zone = _zone.No + ". " + _zone.Name;
            }
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

        string zone;
        public string Zone
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
