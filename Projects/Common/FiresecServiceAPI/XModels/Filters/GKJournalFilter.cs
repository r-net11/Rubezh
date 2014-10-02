using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKJournalFilter
	{
		public GKJournalFilter()
		{
			LastRecordsCount = 100;
			StateClasses = new List<XStateClass>();
			EventNames = new List<string>();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int LastRecordsCount { get; set; }

		[DataMember]
		public List<XStateClass> StateClasses { get; set; }

		[DataMember]
		public List<string> EventNames { get; set; }
	}
}