using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace DeviceLibrary
{
    [Serializable]
    public class DeviceManager
    {
        public List<Device> Devices { get; set; }
    }
}
