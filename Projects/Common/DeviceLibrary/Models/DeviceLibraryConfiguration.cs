using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DeviceLibrary.Models
{
    [DataContract]
    [Serializable]
    public class DeviceLibraryConfiguration
    {
        [DataMember]
        public List<Device> Devices { get; set; }
    }
}