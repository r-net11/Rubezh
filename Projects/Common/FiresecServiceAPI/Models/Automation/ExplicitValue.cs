using System.Xml.Serialization;
using System;
using System.Runtime.Serialization;
using StrazhAPI.Automation.Enums;
using StrazhAPI.GK;
using StrazhAPI.Journal;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using AccessState = StrazhAPI.Automation.Enums.AccessState;

namespace StrazhAPI.Automation
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
		public AccessState? AccessStateValue { get; set; }

		[DataMember]
		public DoorStatus? DoorStatusValue { get; set; }

		[DataMember]
		public BreakInStatus? BreakInStatusValue { get; set; }

		[DataMember]
		public ConnectionStatus? ConnectionStatusValue { get; set; }
	}
}