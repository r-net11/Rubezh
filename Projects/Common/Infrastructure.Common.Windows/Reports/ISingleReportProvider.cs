using CodeReason.Reports;

namespace Infrastructure.Common.Reports
{
	public interface ISingleReportProvider : IReportProvider
	{
		ReportData GetData();
	}
}
