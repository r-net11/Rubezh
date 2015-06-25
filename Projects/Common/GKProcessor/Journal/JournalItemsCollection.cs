using System;
using System.Collections.Generic;
using FiresecAPI.Journal;

namespace GKProcessor
{
	public class JournalItemsCollection
	{
		public DateTime CreationDateTime { get; set; }
		public int? RecordCount { get; set; }
		public List<JournalItem> JournalItems { get; set; }
		public string GkIP { get; set; }
	}
}