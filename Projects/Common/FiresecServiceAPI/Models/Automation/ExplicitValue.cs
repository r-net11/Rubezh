using System;
using System.Drawing;
using System.Runtime.Serialization;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.Models;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ExplicitValue
	{
		public ExplicitValue()
		{
			DateTimeValue = DateTime.Now;
			IntValue = 0;
			StringValue = "";
			ColorValue = KnownColor.Black;
			UidValue = Guid.Empty;
		}

		[DataMember]
		public int IntValue { get; set; }

		[DataMember]
		public bool BoolValue { get; set; }

		[DataMember]
		public DateTime DateTimeValue { get; set; }

		[DataMember]
		public Guid UidValue { get; set; }

		[DataMember]
		public string StringValue { get; set; }

		[DataMember]
		public XStateClass StateTypeValue { get; set; }

		[DataMember]
		public GKDriverType DriverTypeValue { get; set; }

		[DataMember]
		public PermissionType PermissionTypeValue { get; set; }

		[DataMember]
		public JournalEventNameType JournalEventNameTypeValue { get; set; }
		
		[DataMember]
		public JournalEventDescriptionType JournalEventDescriptionTypeValue { get; set; }

		[DataMember]
		public JournalObjectType JournalObjectTypeValue { get; set; }

		[DataMember]
		public KnownColor ColorValue { get; set; }
	}
}