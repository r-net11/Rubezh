using SKDDriver.DataClasses;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GKImitator.Processor
{
	[DataContract]
	public class ImitatorSerializedCollection
	{
		public ImitatorSerializedCollection()
		{
			ImitatorJournalItems = new List<ImitatorJournalItem>();
		}

		[DataMember]
		public List<ImitatorJournalItem> ImitatorJournalItems { get; set; }
	}
}