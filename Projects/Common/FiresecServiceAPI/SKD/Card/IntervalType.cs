using System.ComponentModel;

namespace FiresecAPI
{
	public enum IntervalType
	{
		[DescriptionAttribute("Временные зоны")]
		Time,
		[DescriptionAttribute("Недельные графики")]
		Weekly,
		[DescriptionAttribute("Скользящие посуточные графики")]
		SlideDay,
		[DescriptionAttribute("Скользящие понедельные графики")]
		SlideWeekly
	}
}