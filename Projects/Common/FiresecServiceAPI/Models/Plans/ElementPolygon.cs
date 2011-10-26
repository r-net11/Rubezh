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
    }
}
