using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ElementSubPlan : ElementBasePolygon
    {
        public Plan Plan { get; set; }

        [DataMember]
        public Guid PlanUID { get; set; }

        [DataMember]
        public string Caption { get; set; }

        public override ElementBase Clone()
        {
            ElementBase elementBase = new ElementSubPlan()
            {
                Plan = Plan,
                PlanUID = PlanUID,
                Caption = Caption
            };
            Copy(elementBase);
            return elementBase;
        }
    }
}
