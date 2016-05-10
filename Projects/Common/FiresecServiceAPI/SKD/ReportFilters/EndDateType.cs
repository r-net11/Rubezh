using System.ComponentModel;
using LocalizationConveters;

namespace StrazhAPI.SKD.ReportFilters
{
	public enum EndDateType
	{
		//[Description("Сутки")]
        [LocalizedDescription(typeof(Resources.Language.SKD.ReportFilters.EndDateType), "Day")]
		Day,

        //[Description("Неделя")]
        [LocalizedDescription(typeof(Resources.Language.SKD.ReportFilters.EndDateType), "Week")]
		Week,

        //[Description("Месяц")]
        [LocalizedDescription(typeof(Resources.Language.SKD.ReportFilters.EndDateType), "Month")]
		Month,

        //[Description("Произвольный период")]
        [LocalizedDescription(typeof(Resources.Language.SKD.ReportFilters.EndDateType), "Arbitrary")]
		Arbitrary
	}
}