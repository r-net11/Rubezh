using System.ComponentModel;
using Localization;

namespace FiresecAPI.SKD
{
	public enum IntervalType
	{
        //[DescriptionAttribute("Дневные графики")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Card.IntervalType), "Time")]
		Time,

        //[DescriptionAttribute("Недельные графики")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Card.IntervalType), "Weekly")]
		Weekly,

        //[DescriptionAttribute("Суточные графики")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Card.IntervalType), "SlideDay")]
		SlideDay,

        //[DescriptionAttribute("Скользящие понедельные графики")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Card.IntervalType), "SlideWeekly")]
		SlideWeekly
	}
}