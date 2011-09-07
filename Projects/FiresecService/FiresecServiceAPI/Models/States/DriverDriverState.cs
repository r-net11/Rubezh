using System.Runtime.Serialization;
using System;

namespace FiresecAPI.Models
{
    [DataContract]
    public class DeviceDriverState
    {
        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public DateTime? Time { get; set; }

        [DataMember]
        public string Code { get; set; }

        public DriverState DriverState { get; set; }
    }
}
