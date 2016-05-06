using System.ComponentModel;

namespace StrazhAPI.SKD
{
	public enum SKDDayOfWeek
	{
		[Description("Воскресенье")]
		Sunday = 1,

		[Description("Понедельник")]
		Monday = 2,

		[Description("Вторник")]
		Tuesday = 3,

		[Description("Среда")]
		Wednesday = 4,

		[Description("Четверг")]
		Thursday = 5,

		[Description("Пятница")]
		Friday = 6,

		[Description("Суббота")]
		Saturday = 7,
	}
}