using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecAPI.Journal;

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
		public List<FiresecAPI.Journal.JournalItem> GlobalJournalItems { get; set; }

		[DataMember]
		public GKCallbackResult GKCallbackResult { get; set; }

		[DataMember]
		public GKProgressCallback GKProgressCallback { get; set; }

		[DataMember]
		public List<FiresecAPI.GK.JournalItem> JournalItems { get; set; }

		[DataMember]
		public SKDCallbackResult SKDCallbackResult { get; set; }
	}

	public enum CallbackResultType
	{
		GKProgress,
		GKObjectStateChanged,
		SKDObjectStateChanged,
		NewEvents,
		SKDArchiveCompleted,
		GKArchiveCompleted,
		ConfigurationChanged,
		Disconnecting
	}
}