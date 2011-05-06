using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using DeviceLibrary.Models;

namespace DeviceLibrary
{
    [Serializable]
    public class State
    {
        [XmlAttribute]
        public string Id { get; set; }
        [XmlAttribute]
        public bool IsAdditional { get; set; }
        public List <Frame> Frames { get; set; }
    }
}
