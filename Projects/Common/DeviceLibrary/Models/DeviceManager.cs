using System;
using System.Collections.Generic;

namespace DeviceLibrary.Models
{
    [Serializable]
    public class DeviceManager
    {
        public List<Device> Devices { get; set; }
    }
}