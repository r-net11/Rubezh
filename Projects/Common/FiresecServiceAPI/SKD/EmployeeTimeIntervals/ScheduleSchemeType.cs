using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public enum ScheduleSchemeType
	{
		[DescriptionAttribute("Недельная")]
		Week,

		[DescriptionAttribute("Сменная")]
		SlideDay,

		[DescriptionAttribute("Месячная")]
		Month
	}
}