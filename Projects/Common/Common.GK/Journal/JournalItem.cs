using System;
using XFiresecAPI;
using System.Runtime.Serialization;

namespace Common.GK
{
	[DataContract]
	public class JournalItem
	{
		[DataMember]
		public JournalItemType JournalItemType { get; set; }
		[DataMember]
		public DateTime DeviceDateTime { get; set; }
		[DataMember]
		public DateTime SystemDateTime { get; set; }
		[DataMember]
		public Guid ObjectUID { get; set; }
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public JournalYesNoType YesNo { get; set; }
		[DataMember]
		public string Description { get; set; }
		[DataMember]
		public int ObjectState { get; set; }
		[DataMember]
		public ushort GKObjectNo { get; set; }
		[DataMember]
		public string GKIpAddress { get; set; }
		[DataMember]
		public int? GKJournalRecordNo { get; set; }
		[DataMember]
		public XStateClass StateClass { get; set; }
		[DataMember]
		public string UserName { get; set; }

		public InternalJournalItem InternalJournalItem { get; set; }
	}
}