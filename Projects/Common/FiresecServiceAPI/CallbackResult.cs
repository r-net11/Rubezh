﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.Models;
using XFiresecAPI;
using System;

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
		public List<JournalRecord> JournalRecords { get; set; }

		[DataMember]
		public GKCallbackResult GKCallbackResult { get; set; }

		[DataMember]
		public GKProgressCallback GKProgressCallback { get; set; }

		[DataMember]
		public List<JournalItem> JournalItems { get; set; }

		[DataMember]
		public SKDCallbackResult SKDCallbackResult { get; set; }
	}

	public enum CallbackResultType
	{
		GKProgress,
		GKObjectStateChanged,
		SKDObjectStateChanged,
		NewEvents,
		ArchiveCompleted,
		GKArchiveCompleted,
		SKDArchiveCompleted,
		ConfigurationChanged,
		Disconnecting
	}
}