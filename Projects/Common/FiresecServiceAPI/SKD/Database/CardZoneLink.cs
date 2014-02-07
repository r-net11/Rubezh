using System;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace FiresecAPI
{
	[DataContract]
	public class CardZoneLink
	{
		public CardZoneLink()
		{
			Uid = Guid.NewGuid();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public Guid? CardUid { get; set; }

		[DataMember]
		public Guid? ZoneUid { get; set; }

		[DataMember]
		public bool? IsWithEscort { get; set; }

		[DataMember]
		public IntervalType IntervalType { get; set; }

		[DataMember]
		public Guid? IntervalUid { get; set; }
	}

	[DataContract]
	public enum IntervalType
	{
		[DescriptionAttribute("Временные зоны")]
		Time,

		[DescriptionAttribute("Недельные графики")]
		Weekly,

		[DescriptionAttribute("Скользящие посуточные графики")]
		SlideDay,

		[DescriptionAttribute("Скользящие понедельные графики")]
		SlideWeekly
	}
}