using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using FiresecAPI.Models;

namespace DeviceLibrary.Models
{
    [Serializable]
    public class State
    {
        [XmlAttribute]
        public StateType StateType { get; set; }

        [XmlAttribute]
        public string Code { get; set; }

        public List<Frame> Frames { get; set; }
    }
}