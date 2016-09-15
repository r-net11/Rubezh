using System.ComponentModel;
using Localization.Common.InfrastructureCommon;
using Localization.Converters;

namespace Infrastructure.Common.SKDReports
{
	public enum SKDReportGroup
	{
		[LocalizedDescription(typeof(CommonResources), "Configuration")]
		//[DescriptionAttribute("Конфигурация")]
		Configuration = 100,

		[LocalizedDescription(typeof(CommonResources), "Events")]
		//[DescriptionAttribute("События")]
		Events = 200,

		[LocalizedDescription(typeof(CommonResources), "HR")]
		//[DescriptionAttribute("Картотека")]
		HR = 300,

		[LocalizedDescription(typeof(CommonResources), "TimeTracking")]
		//[DescriptionAttribute("Учет рабочего времени")]
		TimeTracking = 400,
	}
}