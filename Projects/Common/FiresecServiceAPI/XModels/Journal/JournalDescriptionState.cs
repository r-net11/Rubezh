using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XFiresecAPI
{
    public class JournalDescriptionState
    {
        public JournalDescriptionState(string name, XStateClass stateClass, string description)
        {
            Name = name;
            StateClass = stateClass;
            Description = description;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public XStateClass StateClass { get; private set; }
    }
}
