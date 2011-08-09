using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class Instruction
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public StateType StateType { get; set; }

        [DataMember]
        public string Text { get; set; }
    }
}
