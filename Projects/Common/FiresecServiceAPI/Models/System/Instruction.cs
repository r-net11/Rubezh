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
            UID = Guid.NewGuid();
			ZoneUIDs = new List<Guid>();
            Devices = new List<Guid>();
            No = 1;
            Name = "";
            Text = "";
        }

        [DataMember]
        public Guid UID { get; set; }

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
		public List<Guid> ZoneUIDs { get; set; }

        [DataMember]
        public List<Guid> Devices { get; set; }
    }
}