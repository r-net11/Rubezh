using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ZoneLogic
    {
        public ZoneLogic()
        {
            Clauses = new List<Clause>();
            JoinOperator = ZoneLogicJoinOperator.Or;
        }

        [DataMember]
        public List<Clause> Clauses { get; set; }

        [DataMember]
        public ZoneLogicJoinOperator JoinOperator { get; set; }
    }
}