using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.GK;

namespace FiresecAPI.Events
{
	public class EventDescriptionAttribute : Attribute
	{
		public EventDescriptionAttribute(GlobalSubsystemType subsystemType, string name, XStateClass stateClass)
		{
			SubsystemType = subsystemType;
			Name = name;
			StateClass = stateClass;
		}

		public GlobalSubsystemType SubsystemType { get; set; }
		public string Name { get; set; }
		public XStateClass StateClass { get; set; }
	}
}