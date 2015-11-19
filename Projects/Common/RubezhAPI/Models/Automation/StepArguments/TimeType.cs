using System.ComponentModel;

namespace RubezhAPI.Automation
{
	public enum TimeType
	{
		[Description("секунд")]
		Sec = 1,

		[Description("минут")]
		Min = 60,

		[Description("часов")]
		Hour = Min * 60,

		[Description("дней")]
		Day = Hour * 24
	}
}