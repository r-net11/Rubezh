using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GroupControllerModule.Models
{
    [DataContract]
    public class XClause
    {
        public XClause()
        {
            Devices = new List<Guid>();
            Zones = new List<short>();
        }

        [DataMember]
        public List<Guid> Devices { get; set; }

        [DataMember]
        public List<short> Zones { get; set; }

        [DataMember]
        public ClauseOperandType ClauseOperandType { get; set; }

        [DataMember]
        public ClauseOperationType ClauseOperationType { get; set; }

        [DataMember]
        public ClauseJounOperationType ClauseJounOperationType { get; set; }
    }
}