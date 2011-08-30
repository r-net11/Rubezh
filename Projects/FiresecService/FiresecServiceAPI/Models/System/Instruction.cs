using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    public enum InstructionType
    {
        General, 
        Details
    }

    [DataContract]
    public class Instruction
    {
        [DataMember]
        public string No { get; set; }

        [DataMember]
        public StateType StateType { get; set; }

        [DataMember]
        public InstructionType InstructionType { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public List<string> InstructionZonesList { get; set; }

        [DataMember]
        public List<string> InstructionDevicesList { get; set; }
    }
}
