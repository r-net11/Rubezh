using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using LocalizationConveters;

namespace FiresecAPI.SKD
{
	[DataContract]
	[Flags]
	public enum ScheduleSchemeType
	{
		//[DescriptionAttribute("Недельный")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.ScheduleSchemeType), "Week")]
		[EnumMember]
		Week = 1,

        //[DescriptionAttribute("Месячный")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.ScheduleSchemeType), "Month")]
		[EnumMember]
		Month = 2,

        //[DescriptionAttribute("Сменный")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.ScheduleSchemeType), "SlideDay")]
		[EnumMember]
		SlideDay = 4,
	}
}