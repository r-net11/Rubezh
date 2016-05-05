using System.ComponentModel;

namespace StrazhAPI.SKD
{
	public enum IntervalType
	{
		[Description("Дневные графики")]
		Time,

		[Description("Недельные графики")]
		Weekly,

		[Description("Суточные графики")]
		SlideDay,

		[Description("Скользящие понедельные графики")]
		SlideWeekly
	}
}