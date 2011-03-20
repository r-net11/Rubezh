using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ServiceVisualizer
{
    public class DeviceViewModel : INotifyPropertyChanged
    {
        public DeviceViewModel()
        {
            Children = new List<DeviceViewModel>();
        }

        string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
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

        public DeviceViewModel Parent { get; set; }

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

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
