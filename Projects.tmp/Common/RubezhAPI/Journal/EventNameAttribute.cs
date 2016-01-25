﻿using System;
using RubezhAPI.GK;

namespace RubezhAPI.Journal
{
	public class EventNameAttribute : Attribute
	{
		public EventNameAttribute(JournalSubsystemType journalSubsystemType, string name, XStateClass stateClass)
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