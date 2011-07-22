using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DeviceLibrary.Models
{
    [Serializable]
    public class State
    {
        [XmlAttribute]
        public string Class { get; set; }

        [XmlAttribute]
        public string Code { get; set; }

        public List<Frame> Frames { get; set; }
    }
}
