using System.Runtime.Serialization;
using System.Windows.Media;
using System;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ElementSubPlan : ElementBasePolygon
    {
        public Plan Parent { get; set; }

        [DataMember]
        public Guid UID { get; set; }

        [DataMember]
        public string Caption { get; set; }

        public override ElementBase Clone()
        {
            ElementBase elementBase = new ElementSubPlan()
            {
                Parent = Parent,
                UID = UID,
                Caption = Caption
            };
            Copy(elementBase);
            return elementBase;
        }
    }
}
