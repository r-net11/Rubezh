using FiresecAPI.Models;

namespace Infrastructure.Common.Reports
{
	public interface IReportProvider
	{
		string Template { get; }
		ReportType ReportType { get; }
		bool IsEnabled { get; }
	}
}
