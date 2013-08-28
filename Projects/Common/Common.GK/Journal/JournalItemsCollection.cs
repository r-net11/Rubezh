using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Common.GK.Journal
{
	[DataContract]
	public class JournalItemsCollection
	{
		public JournalItemsCollection(List<JournalItem> journalItems)
		{
			JournalItems = journalItems;
			FirstIndex = journalItems.OrderBy(x => x.GKJournalRecordNo).FirstOrDefault().GKJournalRecordNo;
			LastIndex = journalItems.OrderBy(x => x.GKJournalRecordNo).LastOrDefault().GKJournalRecordNo;
			CreationDateTime = DateTime.Now;
		}

		public JournalItemsCollection()
		{
			;
		}

		[DataMember]
		public DateTime CreationDateTime { get; set; }

		[DataMember]
		public int? FirstIndex { get; set; }

		[DataMember]
		public int? LastIndex { get; set; }

		[DataMember]
		public List<JournalItem> JournalItems { get; set; }
	}
}
