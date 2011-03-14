using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestServiceClient
{
    public class DeviceDescriptor : TestServiceClient.ServiceReference.ComDevice
    {
        public bool Enable { get; set; }
        public DeviceDescriptor()
        {
            Enable = false;
        }
    
    }
}
