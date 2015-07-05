using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.Journal;
using FiresecAPI.SKD;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKCallbackResult
	{
		[DataMember]
		public List<JournalItem> JournalItems { get; set; }

		[DataMember]
		public SKDStates SKDStates { get; set; }

		public GKCallbackResult()
		{
			JournalItems = new List<JournalItem>();
			SKDStates = new SKDStates();
		}
	}
}