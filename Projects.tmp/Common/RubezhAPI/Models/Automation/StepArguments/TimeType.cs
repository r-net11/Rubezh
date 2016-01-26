using System.ComponentModel;

namespace RubezhAPI.Automation
{
	public enum TimeType
	{
		[Description("миллисекунд")]
		Millisec = -1,

		[Description("секунд")]
		Sec = 0,

		[Description("минут")]
		Min = 1,

		[Description("часов")]
		Hour = 2,

		[Description("дней")]
		Day = 3
	}
}