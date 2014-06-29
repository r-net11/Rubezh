using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.GK;

namespace FiresecAPI.Events
{
	public class GlobalEventType
	{
		public GlobalEventType(string name, XStateClass stateClass, string description)
		{
			Name = name;
			StateClass = stateClass;
			Description = description;
		}

		public GlobalSubsystemType GlobalSubsystemType { get; private set; }
		public GlobalEventNameEnum GlobalEventNameEnum { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }
		public XStateClass StateClass { get; private set; }
	}
}