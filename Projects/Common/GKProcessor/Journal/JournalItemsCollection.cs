using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.GK;

namespace GKProcessor
{
	[DataContract]
	public class JournalItemsCollection
	{
		[DataMember]
		public DateTime CreationDateTime { get; set; }

		[DataMember]
		public int? RecordCount { get; set; }

		[DataMember]
		public List<JournalItem> JournalItems { get; set; }

		[DataMember]
		public string GkIP { get; set; }
	}
}