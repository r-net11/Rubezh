using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace DeviceEditor
{
    [Serializable]
    public class State
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Id { get; set; }
        public List <Cadr> Cadrs { get; set; }
    }
}
