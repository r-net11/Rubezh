using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    public enum InstructionType
    {
        General, 
        Zone,
        Device
    }

    [DataContract]
    public class Instruction
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public StateType StateType { get; set; }

        [DataMember]
        public InstructionType InstructionType { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public List<string> InstructionDetailsList { get; set; }
    }
}
