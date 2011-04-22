using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace RubezhDevicesMVVM
{
    [Serializable]
    public class State
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Id { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool IsAdditional { get; set; }
        public List <Frame> Frames { get; set; }
    }
}
