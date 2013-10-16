using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
    [DataContract]
    public class XEvent
    {
        public XEvent(string name, XStateClass stateClass)
        {
            Name = name;
            StateClass = stateClass;
        }

        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public XStateClass StateClass { get; private set; }
    }
}
