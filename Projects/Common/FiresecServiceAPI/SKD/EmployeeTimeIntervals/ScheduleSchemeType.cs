using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.EmployeeTimeIntervals
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