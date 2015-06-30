using System.Collections.Generic;
using System.Runtime.Serialization;
using System;

namespace FS2Api
{
	[DataContract]
	public class FS2JournalItemsCollection
	{
		public FS2JournalItemsCollection()
		{
			FireJournalItems = new List<FS2JournalItem>();
			SecurityJournalItems = new List<FS2JournalItem>();
		}

		[DataMember]
		public DateTime CreationDateTime { get; set; }

		[DataMember]
		public List<FS2JournalItem> FireJournalItems { get; set; }

		[DataMember]
		public List<FS2JournalItem> SecurityJournalItems { get; set; }
	}
}