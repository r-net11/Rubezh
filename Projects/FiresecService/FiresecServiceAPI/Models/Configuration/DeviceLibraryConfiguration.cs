using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class DeviceLibraryConfiguration
    {
        public DeviceLibraryConfiguration()
        {
            Devices = new List<Models.DeviceLibrary.Device>();
        }

        [DataMember]
        public List<Models.DeviceLibrary.Device> Devices { get; set; }
    }
}