using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DeviceLibrary.Models
{
    [Serializable]
    public class Device
    {
        [XmlAttribute]
        public string Id { get; set; }

        public List<State> States { get; set; }
    }
}