using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.Models;
using System;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ExplicitValue
	{
		public ExplicitValue()
		{
			DateTimeValue = DateTime.Now;
			IntValue = 0;
			FloatValue = 0;
			StringValue = "";
			ColorValue = new Color();
			UidValue = Guid.Empty;
		}

		[DataMember]
		public int IntValue { get; set; }

		[DataMember]
		public double FloatValue { get; set; }

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
		public Color ColorValue { get; set; }
	}
}