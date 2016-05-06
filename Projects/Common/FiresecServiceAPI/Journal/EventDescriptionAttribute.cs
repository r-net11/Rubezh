using System;
using System.Collections.Generic;
using System.Linq;

namespace StrazhAPI.Journal
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