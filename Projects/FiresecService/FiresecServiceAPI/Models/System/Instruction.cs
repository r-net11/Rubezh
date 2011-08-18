using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    public enum Cover
    {
        All, 
        Any
    }

    [DataContract]
    public class InstructionZone
    {
        [DataMember]
        public string ZoneNo { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public Cover Cover { get; set; }

        [DataMember]
        public Dictionary<string, string> InstructionDevices { get; set; }
    }

    [DataContract]
    public class Instruction
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public StateType StateType { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public List<InstructionZone> InstructionZones { get; set; }

        [DataMember]
        public Dictionary<string, string> InstructionDevices { get; set; }

        [DataMember]
        public Cover Cover;
    }
}
