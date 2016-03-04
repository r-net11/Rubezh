using System.Xml.Serialization;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.Models;
using System;
using System.Runtime.Serialization;
using System.Windows.Media;
using FiresecAPI.SKD;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ExplicitValue
	{
		private TimeSpan _timeSpanValue;

		public ExplicitValue()
		{
			DateTimeValue = DateTime.Now;
			IntValue = 0;
			StringValue = string.Empty;
			ColorValue = new Color();
			UidValue = Guid.Empty;
		}

		[DataMember]
		public int IntValue { get; set; }

		[DataMember]
		public bool BoolValue { get; set; }

		[DataMember]
		public DateTime DateTimeValue { get; set; }

		[DataMember]
		[XmlIgnore]
		public TimeSpan TimeSpanValue
		{
			get	{ return _timeSpanValue; }
			set { _timeSpanValue = value; }
		}

		[DataMember]
		[XmlElement("TimeSpanValue")]
		public long TimeSpanValueTicks
		{
			get { return _timeSpanValue.Ticks; }
			set { _timeSpanValue = new TimeSpan(value); }
		}

		[DataMember]
		public Guid UidValue { get; set; }

		[DataMember]
		public string StringValue { get; set; }

		[DataMember]
		public XStateClass StateTypeValue { get; set; }

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

		[DataMember]
		public CardType CardTypeValue { get; set; }

		[DataMember]
		public FiresecAPI.Automation.Enums.AccessState? AccessStateValue { get; set; }

		[DataMember]
		public FiresecAPI.Automation.Enums.DoorStatus? DoorStatusValue { get; set; }

		[DataMember]
		public FiresecAPI.Automation.Enums.BreakInStatus? BreakInStatusValue { get; set; }

		[DataMember]
		public FiresecAPI.Automation.Enums.ConnectionStatus? ConnectionStatusValue { get; set; }
	}
}