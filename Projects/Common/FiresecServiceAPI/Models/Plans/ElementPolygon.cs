using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ElementPolygon : ElementBasePolygon, IZIndexedElement
    {
        public ElementPolygon()
        {

        }

        [DataMember]
        public int ZIndex { get; set; }

        public override ElementBase Clone()
        {
            ElementBase elementBase = new ElementPolygon();
            Copy(elementBase);
            return elementBase;
        }
    }
}
