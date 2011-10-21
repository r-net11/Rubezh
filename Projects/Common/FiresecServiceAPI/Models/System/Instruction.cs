using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class Instruction
    {
        [DataMember]
        public ulong No { get; set; }

        [DataMember]
        public StateType StateType { get; set; }

        [DataMember]
        public InstructionType InstructionType { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public List<ulong?> InstructionZonesList { get; set; }

        [DataMember]
        public List<Guid> InstructionDevicesList { get; set; }
    }
}