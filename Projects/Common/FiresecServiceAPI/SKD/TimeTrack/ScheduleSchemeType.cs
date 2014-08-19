using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	[Flags]
	public enum ScheduleSchemeType
	{
		[DescriptionAttribute("Недельная")]
		[EnumMember]
		Week = 1,

		[DescriptionAttribute("Месячная")]
		[EnumMember]
		Month = 2,

		[DescriptionAttribute("Сменная")]
		[EnumMember]
		SlideDay = 4,
	}
}