using System.ComponentModel;
using Localization.Converters;
using Localization.SKD.Common;

namespace SKDModule.Model
{
	public enum TimeTrackingPeriod
	{
		[DescriptionAttribute("Текущую неделю")]
        [LocalizedDescription(typeof(CommonResources), "CurrentWeek")]
		CurrentWeek,

        [DescriptionAttribute("Предыдущую неделю")]
        [LocalizedDescription(typeof(CommonResources), "PreviousWeek")]
		PreviousWeek,

        [DescriptionAttribute("Текущий месяц")]
        [LocalizedDescription(typeof(CommonResources), "CurrentMonth")]
		CurrentMonth,

        [DescriptionAttribute("Предыдущий месяц")]
        [LocalizedDescription(typeof(CommonResources), "PreviousMonth")]
		PreviousMonth,

        [DescriptionAttribute("Период")]
        [LocalizedDescription(typeof(CommonResources), "Period")]
		Period,
	}
}