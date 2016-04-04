using System;
using System.Collections.Generic;
using System.Linq;

namespace RubezhAPI.Journal
{
	public class EventDescriptionAttribute : Attribute
	{
		public EventDescriptionAttribute(string name, JournalEventNameType journalEventNameType)
		{
			Name = name;
			JournalEventNameType = journalEventNameType;
		}

		public string Name { get; set; }
		public JournalEventNameType JournalEventNameType { get; set; }
	}
}