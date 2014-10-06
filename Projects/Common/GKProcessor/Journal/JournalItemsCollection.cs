using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.GK;

namespace GKProcessor
{
	public class JournalItemsCollection
	{
		public DateTime CreationDateTime { get; set; }
		public int? RecordCount { get; set; }
		public List<GKJournalItem> JournalItems { get; set; }
		public string GkIP { get; set; }
	}
}