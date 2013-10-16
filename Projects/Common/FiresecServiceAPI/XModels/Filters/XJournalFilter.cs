using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XJournalFilter
	{
		public XJournalFilter()
		{
			LastRecordsCount = 100;
			StateClasses = new List<XStateClass>();
			EventNames = new List<XEvent>();
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
		public List<XEvent> EventNames { get; set; }
	}
}