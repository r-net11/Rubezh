using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public enum HolidayType
	{
		[DescriptionAttribute("Праздник")]
		[EnumMember]
		Holiday = 0,

		[DescriptionAttribute("Предпраздничный день")]
		[EnumMember]
		BeforeHoliday = 1,

		[DescriptionAttribute("Рабочий выходной")]
		[EnumMember]
		WorkingHoliday = 2,
	}
}