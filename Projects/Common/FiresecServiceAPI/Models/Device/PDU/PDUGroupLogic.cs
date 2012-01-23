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

        public override string ToString()
        {
            if (Devices.Count > 0)
                return "Выбрано устройств: " + Devices.Count.ToString();
            return "";
        }
    }
}