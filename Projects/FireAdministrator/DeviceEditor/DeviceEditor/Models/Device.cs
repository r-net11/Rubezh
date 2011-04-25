using System.Collections.Generic;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace DeviceEditor
{
    [Serializable]
    public class Device
    {
        [XmlAttribute]
        public string Id { get; set; }
        public List<State> States { get; set; }
    }
}
