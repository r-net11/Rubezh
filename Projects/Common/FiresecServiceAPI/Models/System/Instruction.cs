using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class Instruction
    {
        public Instruction()
        {
			Zones = new List<int>();
            Devices = new List<Guid>();
            No = 1;
            Name = "";
            Text = "";
        }

        [DataMember]
		public int No { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public StateType StateType { get; set; }

        [DataMember]
        public InstructionType InstructionType { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
		public List<int> Zones { get; set; }

        [DataMember]
        public List<Guid> Devices { get; set; }
    }
}