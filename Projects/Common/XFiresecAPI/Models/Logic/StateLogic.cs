using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
    [DataContract]
    public class StateLogic
    {
        public StateLogic()
        {
            Clauses = new List<XClause>();
        }

        [DataMember]
        public XStateType StateType { get; set; }

        [DataMember]
        public List<XClause> Clauses { get; set; }
    }
}