using System.ComponentModel;

namespace StrazhAPI.SKD.ReportFilters
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