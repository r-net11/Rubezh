using System.ComponentModel;

namespace StrazhAPI.SKD.ReportFilters
{
	public enum ReportPeriodType
	{
		[Description("Последние сутки")]
		Day,

		[Description("Последняя неделя")]
		Week,

		[Description("Последний месяц")]
		Month,

		[Description("Произвольный период")]
		Arbitrary
	}
}