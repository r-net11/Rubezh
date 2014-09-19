using System;
using System.Linq;
using FiresecAPI.GK;
using System.Collections.Generic;

namespace FiresecAPI.Journal
{
	public class EventDescriptionAttribute : Attribute
	{
		public EventDescriptionAttribute(string name, params JournalEventNameType[] journalEventNameTypes)
		{
			Name = name;
			JournalEventNameTypes = journalEventNameTypes.ToList();
		}

		public string Name { get; set; }
		public List<JournalEventNameType> JournalEventNameTypes { get; set; }
	}
}