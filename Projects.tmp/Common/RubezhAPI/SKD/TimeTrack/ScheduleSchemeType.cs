using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace RubezhAPI.SKD
{
	[DataContract]
	[Flags]
	public enum ScheduleSchemeType
	{
		[DescriptionAttribute("Недельный")]
		[EnumMember]
		Week = 1,

		[DescriptionAttribute("Месячный")]
		[EnumMember]
		Month = 2,

		[DescriptionAttribute("Сменный")]
		[EnumMember]
		SlideDay = 4,
	}
}