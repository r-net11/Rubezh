using System.ComponentModel;
using System.Runtime.Serialization;
using LocalizationConveters;

namespace StrazhAPI.SKD
{
	[DataContract]
	public enum HolidayType
	{
		//[DescriptionAttribute("Праздник")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.HolidayType), "Holiday")]
		[EnumMember]
		Holiday = 0,

        //[DescriptionAttribute("Предпраздничный день")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.HolidayType), "BeforeHoliday")]
		[EnumMember]
		BeforeHoliday = 1,

        //[DescriptionAttribute("Рабочий выходной")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.HolidayType), "WorkingHoliday")]
		[EnumMember]
		WorkingHoliday = 2,
	}
}