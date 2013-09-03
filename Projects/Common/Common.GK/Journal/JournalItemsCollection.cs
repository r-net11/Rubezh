using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using XFiresecAPI;

namespace Common.GK.Journal
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
	}
}
