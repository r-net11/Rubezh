using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.Models;
using XFiresecAPI;

namespace FiresecAPI
{
	[DataContract]
	public class CallbackResult
	{
		[DataMember]
		public CallbackResultType CallbackResultType { get; set; }

		[DataMember]
		public List<JournalRecord> JournalRecords { get; set; }

		[DataMember]
		public List<JournalItem> JournalItems { get; set; }

		[DataMember]
		public GKCallbackResult GKCallbackResult { get; set; }
	}

	public enum CallbackResultType
	{
		NewGKEvents,
		GKObjectStateChanged,
		NewEvents,
		ArchiveCompleted,
		ConfigurationChanged,
		Disconnecting
	}
}