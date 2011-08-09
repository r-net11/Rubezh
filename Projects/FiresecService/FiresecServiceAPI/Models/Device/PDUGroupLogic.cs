using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class PDUGroupLogic
    {
        public PDUGroupLogic()
        {
            Devices = new List<PDUGroupDevice>();
        }

        [DataMember]
        public List<PDUGroupDevice> Devices { get; set; }

        [DataMember]
        public bool AMTPreset { get; set; }
    }

    [DataContract]
    public class PDUGroupDevice
    {
        [DataMember]
        public string DeviceUID { get; set; }

        [DataMember]
        public bool IsInversion { get; set; }

        [DataMember]
        public int OnDelay { get; set; }

        [DataMember]
        public int OffDelay { get; set; }
    }
}