using System.ComponentModel;
using LocalizationConveters;

namespace FiresecAPI.SKD.ReportFilters
{
	public enum ReportPeriodType
	{
		//[Description("Последние сутки")]
        [LocalizedDescription(typeof(Resources.Language.SKD.ReportFilters.ReportPeriodType), "Day")]
		Day,

        //[Description("Последняя неделя")]
        [LocalizedDescription(typeof(Resources.Language.SKD.ReportFilters.ReportPeriodType), "Week")]
		Week,

        //[Description("Последний месяц")]
        [LocalizedDescription(typeof(Resources.Language.SKD.ReportFilters.ReportPeriodType), "Month")]
		Month,

        //[Description("Произвольный период")]
        [LocalizedDescription(typeof(Resources.Language.SKD.ReportFilters.ReportPeriodType), "Arbitrary")]
		Arbitrary
	}
}