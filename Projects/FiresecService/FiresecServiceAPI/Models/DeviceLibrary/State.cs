using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models.DeviceLibrary
{
    [DataContract]
    public class State
    {
        [DataMember]
        public StateType StateType { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public List<Frame> Frames { get; set; }
    }
}