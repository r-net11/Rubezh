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
			ImitatorUsers = new List<ImitatorUser>();
		}

		[DataMember]
		public List<ImitatorJournalItem> ImitatorJournalItems { get; set; }

		[DataMember]
		public List<ImitatorUser> ImitatorUsers { get; set; }
	}
}