using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
    [DataContract]
    public class XClause
    {
        public XClause()
        {
            StateType = XStateType.Fire1;
            Devices = new List<Guid>();
            Zones = new List<short>();
        }

        [DataMember]
        public XStateType StateType { get; set; }

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