using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace GKImitator.Processor
{
	[DataContract]
	public class ImitatorJournalItemCollection
	{
		public ImitatorJournalItemCollection()
		{
			ImitatorJournalItems = new List<ImitatorJournalItem>();
		}

		[DataMember]
		public List<ImitatorJournalItem> ImitatorJournalItems { get; set; }
	}
}