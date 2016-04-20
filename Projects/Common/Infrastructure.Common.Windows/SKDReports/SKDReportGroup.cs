using System.ComponentModel;

namespace Infrastructure.Common.Windows.SKDReports
{
	public enum SKDReportGroup
	{
		[DescriptionAttribute("Конфигурация")]
		Configuration = 100,
		[DescriptionAttribute("События")]
		Events = 200,
		[DescriptionAttribute("Картотека")]
		HR = 300,
		[DescriptionAttribute("Учет рабочего времени")]
		TimeTracking = 400,
	}
}