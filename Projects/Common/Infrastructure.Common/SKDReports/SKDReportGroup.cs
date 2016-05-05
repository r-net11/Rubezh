using System.ComponentModel;
using LocalizationConveters;

namespace Infrastructure.Common.SKDReports
{
	public enum SKDReportGroup
	{
		//[DescriptionAttribute("Конфигурация")]
        [LocalizedDescription(typeof(Resources.Language.SKDReports.SKDReportGroup), "Configuration")]
		Configuration = 100,

        //[DescriptionAttribute("События")]
        [LocalizedDescription(typeof(Resources.Language.SKDReports.SKDReportGroup), "Events")]
		Events = 200,

        //[DescriptionAttribute("Картотека")]
        [LocalizedDescription(typeof(Resources.Language.SKDReports.SKDReportGroup), "HR")]
		HR = 300,

        //[DescriptionAttribute("Учет рабочего времени")]
        [LocalizedDescription(typeof(Resources.Language.SKDReports.SKDReportGroup), "TimeTracking")]
		TimeTracking = 400,
	}
}