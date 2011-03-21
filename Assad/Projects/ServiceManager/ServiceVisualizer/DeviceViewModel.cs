using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Common;
using ClientApi;

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
            Zone = device.Zone;
            Description = device.Description;
            State = device.State;
            States = "";
            foreach (string state in device.States)
            {
                States += state + "\n";
            }
            Parameters = "";
            foreach (ServiceApi.Parameter parameter in device.Parameters)
            {
                Parameters += parameter.Caption + " - " + parameter.Value + "\n";
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

        string states;
        public string States
        {
            get { return states; }
            set
            {
                states = value;
                OnPropertyChanged("States");
            }
        }

        string parameters;
        public string Parameters
        {
            get { return parameters; }
            set
            {
                parameters = value;
                OnPropertyChanged("Parameters");
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
                ViewModel.Current.SelectedDevicesViewModel = this;
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
