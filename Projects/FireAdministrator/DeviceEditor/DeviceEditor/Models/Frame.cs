using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeviceEditor
{
    [Serializable]
    public class Frame
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int Id { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int Duration { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int Layer { get; set;}
        public string Image { get; set; }
    }
}
