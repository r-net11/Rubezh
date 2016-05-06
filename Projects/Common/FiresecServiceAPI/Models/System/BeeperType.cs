using System.ComponentModel;

namespace StrazhAPI.Models
{
	public enum BeeperType
	{
		[Description("<нет>")]
		None = 0,

		[Description("Тревога")]
		Alarm = 200,

		[Description("Внимание")]
		Attention = 50
	}
}