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
		public List<JournalItem> JournalItems { get; set; }

		[DataMember]
		public List<JournalRecord> JournalRecords { get; set; }
	}

	public enum CallbackResultType
	{
		NewGKEvents,
		NewEvents,
		ArchiveCompleted,
		ConfigurationChanged,
		Disconnecting
	}
}