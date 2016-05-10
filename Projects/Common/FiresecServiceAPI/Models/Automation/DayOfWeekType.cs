using System.ComponentModel;
using LocalizationConveters;

namespace StrazhAPI.Automation
{
	public enum DayOfWeekType
	{
		//[DescriptionAttribute("Любой")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.DayOfWeekType), "Any")]
		Any,

		//[DescriptionAttribute("Понедельник")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.DayOfWeekType), "Monday")]
        Monday,

		//[DescriptionAttribute("Вторник")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.DayOfWeekType), "Tuesday")]
        Tuesday,

		//[DescriptionAttribute("Среда")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.DayOfWeekType), "Wednesday")]
        Wednesday,

		//[DescriptionAttribute("Четверг")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.DayOfWeekType), "Thursday")]
        Thursday,

		//[DescriptionAttribute("Пятница")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.DayOfWeekType), "Friday")]
        Friday,

		//[DescriptionAttribute("Суббота")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.DayOfWeekType), "Saturday")]
        Saturday,

		//[DescriptionAttribute("Воскресенье")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.DayOfWeekType), "Sunday")]
        Sunday
	}
}