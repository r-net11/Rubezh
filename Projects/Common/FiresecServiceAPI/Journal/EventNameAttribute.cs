using System;
using StrazhAPI.GK;

namespace StrazhAPI.Journal
{
	public class EventNameAttribute : Attribute
	{
		public EventNameAttribute(JournalSubsystemType journalSubsystemType, string name, XStateClass stateClass, string nameInFilter = null)
		{
			JournalSubsystemType = journalSubsystemType;
			Name = name;
			StateClass = stateClass;
			NameInFilter = nameInFilter ?? name;
		}

		public JournalSubsystemType JournalSubsystemType { get; set; }

		public string Name { get; set; }

		public XStateClass StateClass { get; set; }

		public string NameInFilter { get; set; }
	}
}