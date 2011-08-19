using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models.DeviceLibrary
{
    [DataContract]
    public class Device
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public List<State> States { get; set; }
    }
}