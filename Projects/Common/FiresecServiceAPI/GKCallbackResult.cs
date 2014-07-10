using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKCallbackResult
	{
		[DataMember]
		public List<XJournalItem> JournalItems { get; set; }

		[DataMember]
		public GKStates GKStates { get; set; }

		public GKCallbackResult()
		{
			JournalItems = new List<XJournalItem>();
			GKStates = new GKStates();
		}
	}
}