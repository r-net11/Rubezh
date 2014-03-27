using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public enum EmployeeHolidayType
	{
		[DescriptionAttribute("Праздник")]
		Holiday,

		[DescriptionAttribute("Предпраздничный день")]
		BeforeHoliday,

		[DescriptionAttribute("Рабочий выходной")]
		WorkingHoliday
	}
}