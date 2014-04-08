using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public enum IntervalTransitionType
	{
		[DescriptionAttribute("Нет")]
		Day,

		[DescriptionAttribute("Переход")]
		Night,

		[DescriptionAttribute("Следующий день")]
		NextDay
	}
}