using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RubezhAX
{
    public class DeviceDescriptor : RubezhAX.ServiceReference.ComDevice
    {
        public bool Enable { get; set; }
        public DeviceDescriptor()
        {
            Enable = false;
        }
    
    }
}
