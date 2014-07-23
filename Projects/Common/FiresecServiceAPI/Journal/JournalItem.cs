using System;
using System.Runtime.Serialization;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using System.Collections.Generic;

namespace FiresecAPI.Journal
{
	[DataContract]
	public class JournalItem : SKDModelBase
	{
		public JournalItem():base()
		{
			DeviceDateTime = DateTime.Now;
			SystemDateTime = DateTime.Now;
			StateClass = XStateClass.Norm;
			JournalDetalisationItems = new List<JournalDetalisationItem>();
		}

		[DataMember]
		public DateTime SystemDateTime { get; set; }

		[DataMember]
		public DateTime DeviceDateTime { get; set; }
		
		[DataMember]
		public JournalEventNameType JournalEventNameType { get; set; }

		[DataMember]
		public JournalEventDescriptionType JournalEventDescriptionType { get; set; }

		[DataMember]
		public string NameText { get; set; }

		[DataMember]
		public string DescriptionText { get; set; }

		[DataMember]
		public XStateClass StateClass { get; set; }

		[DataMember]
		public JournalSubsystemType JournalSubsystemType { get; set; }

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
		public List<JournalDetalisationItem> JournalDetalisationItems { get; set; }
	}
}