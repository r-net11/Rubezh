using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	[Flags]
	public enum ScheduleSchemeType
	{
		[Description("Недельный")]
		[EnumMember]
		Week = 1,

		[Description("Месячный")]
		[EnumMember]
		Month = 2,

		[Description("Сменный")]
		[EnumMember]
		SlideDay = 4,
	}
}