using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceApi;
using System.ComponentModel;

namespace RubezhAX
{
    public class DeviceDescriptor : INotifyPropertyChanged
    {

       public ViewModel RootClass { get; set; }
       public string DriverId;
       public string Address;
       public string DriverName;
       public List<string> LastEvents;
       public string State;
       public List<State> States;
       public List<string> Zones;
       public string Path;
        
        
        public bool Enable { get; set; }
        public string DeviceName { get; set; }
        public DeviceDescriptor()
        {
            Enable = false;
        }

        DeviceDescriptor parent;
        public DeviceDescriptor Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                if (parent != null)
                {
                    parent.Children.Add(this);
                }
            }
        }

        List<DeviceDescriptor> children;
        public List<DeviceDescriptor> Children
        {
            get
            {
                if (children == null)
                    children = new List<DeviceDescriptor>();
                return children;
            }
            set
            {
                children = value;
            }
        }

        private bool isSelected;

        #region IsSelected

        public bool IsSelected
        {
            get
            {
                //                SelectedDevice = this.

                return isSelected;
            }

            set
            {
                if (value != isSelected)
                {
                    isSelected = value;
                    RootClass.SelectedDevice = this;
                    this.OnPropertyChanged("IsSelected");
                }

            }


        }
        #endregion 
    
        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;



    }
}
