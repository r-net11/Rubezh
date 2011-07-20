using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DeviceLibrary.Models
{
    [Serializable]
    public class State
    {
        [XmlAttribute]
        public string Id { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public bool IsAdditional { get; set; }

        public List<Frame> Frames { get; set; }
    }
}
