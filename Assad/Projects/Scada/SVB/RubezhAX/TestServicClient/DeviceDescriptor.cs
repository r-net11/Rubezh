using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceApi;

namespace TestServiceClient
{
    public class DeviceDescriptor 
    {
       
       public string DriverId;
       public string Address;
       public string DriverName;
       public string State;
       public List<string> States;
       public string Zone;
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
    



    }
}
