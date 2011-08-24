using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class DeviceDriverState
    {
        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public string Code { get; set; }

        public DriverState DriverState { get; set; }
    }
}
