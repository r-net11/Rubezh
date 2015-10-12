using System.ComponentModel;

namespace RubezhAPI.Automation
{
	public enum DayOfWeekType
	{
		[DescriptionAttribute("Любой")]
		Any,

		[DescriptionAttribute("Понедельник")]
		Monday,

		[DescriptionAttribute("Вторник")]
		Tuesday,

		[DescriptionAttribute("Среда")]
		Wednesday,

		[DescriptionAttribute("Четверг")]
		Thursday,

		[DescriptionAttribute("Пятница")]
		Friday,

		[DescriptionAttribute("Суббота")]
		Saturday,

		[DescriptionAttribute("Воскресенье")]
		Sunday
	}
}