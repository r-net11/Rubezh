using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.GK;

namespace FiresecAPI.Journal
{
	public class EventDescriptionAttribute : Attribute
	{
		public EventDescriptionAttribute(JournalSubsystemType journalSubsystemType, string name, XStateClass stateClass)
		{
			JournalSubsystemType = journalSubsystemType;
			Name = name;
			StateClass = stateClass;
		}

		public JournalSubsystemType JournalSubsystemType { get; set; }
		public string Name { get; set; }
		public XStateClass StateClass { get; set; }
	}
}