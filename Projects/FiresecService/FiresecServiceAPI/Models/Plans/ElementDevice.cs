using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.Models
{
    public class ElementDevice
    {
        [DataMember]
        //[XmlAttribute]
        public double Left { get; set; }
        [DataMember]
        //[XmlAttribute]
        public double Top { get; set; }
        [DataMember]
        //[XmlAttribute]
        public double Width { get; set; }
        [DataMember]
        //[XmlAttribute]
        public double Height { get; set; }
        [DataMember]
        //[XmlAttribute]
        public string Id { get; set; }
    }
}
