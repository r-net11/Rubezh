using System;
using System.Xml.Serialization;

namespace DeviceLibrary.Models
{
    [Serializable]
    public class Frame
    {
        [XmlAttribute]
        public int Id { get; set; }

        [XmlAttribute]
        public int Duration { get; set; }

        [XmlAttribute]
        public int Layer { get; set; }

        public string Image { get; set; }
    }
}
