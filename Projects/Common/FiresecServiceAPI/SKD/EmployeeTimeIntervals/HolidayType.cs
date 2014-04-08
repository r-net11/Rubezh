using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public enum HolidayType
	{
		[DescriptionAttribute("Праздник")]
		Holiday,

		[DescriptionAttribute("Предпраздничный день")]
		BeforeHoliday,

		[DescriptionAttribute("Рабочий выходной")]
		WorkingHoliday
	}
}