using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using FiresecAPI.AutomationCallback;

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
		public SKDProgressCallback SKDProgressCallback { get; set; }

		[DataMember]
		public SKDCallbackResult SKDCallbackResult { get; set; }

		[DataMember]
		public SKDStates SKDStates { get; set; }

		[DataMember]
		public AutomationCallbackResult AutomationCallbackResult { get; set; }
	}

	public enum CallbackResultType
	{
		SKDProgress,
		SKDObjectStateChanged,
		NewEvents,
		ArchiveCompleted,
		AutomationCallbackResult,
		ConfigurationChanged,
		Disconnecting
	}
}