using System.ComponentModel;

namespace SKDModule.Intervals.TimeTracking.Model
{
	public enum TimeTrackingPeriod
	{
		[DescriptionAttribute("Текущую неделю")]
		CurrentWeek,
		[DescriptionAttribute("Предыдущую неделю")]
		PreviosWeek,
		[DescriptionAttribute("Текущий месяц")]
		CurrentMonth,
		[DescriptionAttribute("Предыдущий месяц")]
		PreviosMonth,
		[DescriptionAttribute("Период")]
		Period,
	}
}
