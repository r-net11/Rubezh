﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using FiresecAPI.AutomationCallback;
using FiresecAPI.SKD.Device;

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
		public SKDStates SKDStates { get; set; }

		[DataMember]
		public AutomationCallbackResult AutomationCallbackResult { get; set; }

		[DataMember]
		public List<SKDDeviceSearchInfo> SearchDevices { get; set; }
	}

	public enum CallbackResultType
	{
		GKProgress,
		GKObjectStateChanged,
		SKDObjectStateChanged,
		NewEvents,
		ArchiveCompleted,
		AutomationCallbackResult,
		ConfigurationChanged,
		Disconnecting,
		NewSearchDevices
	}
}