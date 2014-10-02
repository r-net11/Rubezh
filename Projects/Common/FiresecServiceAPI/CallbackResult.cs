using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.SKD;

namespace FiresecAPI
{
	[DataContract]
	public class CallbackResult
	{
		[DataMember]
		public Guid ArchivePortionUID { get; set; }

		[DataMember]
		public CallbackResultType CallbackResultType { get; set; }

		[DataMember]
		public List<JournalItem> JournalItems { get; set; }

		[DataMember]
		public GKProgressCallback GKProgressCallback { get; set; }

		[DataMember]
		public GKCallbackResult GKCallbackResult { get; set; }

		[DataMember]
		public List<GKJournalItem> GKJournalItemsArchiveCompleted { get; set; }

		[DataMember]
		public SKDStates SKDStates { get; set; }

		[DataMember]
		public AutomationCallbackResult AutomationCallbackResult { get; set; }
	}

	public enum CallbackResultType
	{
		GKProgress,
		GKObjectStateChanged,
		GKArchiveCompleted,
		SKDObjectStateChanged,
		NewEvents,
		ArchiveCompleted,
		AutomationCallbackResult,
		ConfigurationChanged,
		Disconnecting
	}
}