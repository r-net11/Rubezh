using System.Collections.Generic;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace RubezhDevicesMVVM
{
    [Serializable]
    public class Device
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Id { get; set; }
        public List<State> States { get; set; }
    }
}
