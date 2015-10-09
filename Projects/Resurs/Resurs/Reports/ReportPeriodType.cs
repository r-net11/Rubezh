using System.ComponentModel;

namespace Resurs.Reports
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