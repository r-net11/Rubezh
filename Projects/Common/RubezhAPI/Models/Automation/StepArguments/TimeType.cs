using System.ComponentModel;

namespace RubezhAPI.Automation
{
	public enum TimeType
	{
		[Description("миллисекунд")]
		Millisecond = -1,

		[Description("секунд")]
		Second = 0,

		[Description("минут")]
		Minute = 1,

		[Description("часов")]
		Hour = 2,

		[Description("дней")]
		Day = 3
	}
}