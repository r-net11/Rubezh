using System.ComponentModel;

namespace SKDModule
{
	public enum EndDateType
	{
		[Description("Сутки")]
		Day,

		[Description("Неделя")]
		Week,

		[Description("Месяц")]
		Month,

		[Description("Произвольный период")]
		Arbitrary
	}
}
