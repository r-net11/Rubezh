using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using StrazhAPI.SKD;

namespace StrazhAPI.Journal
{
	[DataContract]
	public class JournalItem : SKDModelBase
	{
		public JournalItem()
		{
			SystemDateTime = DateTime.Now;
			JournalDetalisationItems = new List<JournalDetalisationItem>();
			JournalObjectType = JournalObjectType.None;
		}

		[DataMember]
		public JournalItemType JournalItemType { get; set; }

		[DataMember]
		public DateTime SystemDateTime { get; set; }

		[DataMember]
		public DateTime? DeviceDateTime { get; set; }

		[DataMember]
		public JournalSubsystemType JournalSubsystemType { get; set; }

		[DataMember]
		public JournalEventNameType JournalEventNameType { get; set; }

		[DataMember]
		public JournalEventDescriptionType JournalEventDescriptionType { get; set; }

		[DataMember]
		public string DescriptionText { get; set; }

		[DataMember]
		public JournalObjectType JournalObjectType { get; set; }

		[DataMember]
		public Guid ObjectUID { get; set; }

		[DataMember]
		public string ObjectName { get; set; }

		[DataMember]
		public string UserName { get; set; }

		[DataMember]
		public int CardNo { get; set; }

		[DataMember]
		public Guid EmployeeUID { get; set; }

		[DataMember]
		public Guid VideoUID { get; set; }

		[DataMember]
		public Guid CameraUID { get; set; }

		[DataMember]
		public JournalErrorCode ErrorCode { get; set; }

		[DataMember]
		public Guid ControllerUID { get; set; }

		[DataMember]
		public List<JournalDetalisationItem> JournalDetalisationItems { get; set; }
	}

	public enum JournalItemType
	{
		Online = 0,
		Offline = 1
	}
}