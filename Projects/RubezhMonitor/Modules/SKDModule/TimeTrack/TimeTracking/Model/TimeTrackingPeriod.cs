using System.ComponentModel;

namespace SKDModule.Model
{
	public enum TimeTrackingPeriod
	{
		[DescriptionAttribute("Текущую неделю")]
		CurrentWeek,

		[DescriptionAttribute("Предыдущую неделю")]
		PreviousWeek,

		[DescriptionAttribute("Текущий месяц")]
		CurrentMonth,

		[DescriptionAttribute("Предыдущий месяц")]
		PreviousMonth,

		[DescriptionAttribute("Период")]
		Period,
	}
}