using System.ComponentModel;

namespace StrazhAPI.Automation
{
	public enum DayOfWeekType
	{
		[Description("Любой")]
		Any,

		[Description("Понедельник")]
		Monday,

		[Description("Вторник")]
		Tuesday,

		[Description("Среда")]
		Wednesday,

		[Description("Четверг")]
		Thursday,

		[Description("Пятница")]
		Friday,

		[Description("Суббота")]
		Saturday,

		[Description("Воскресенье")]
		Sunday
	}
}