using CodeReason.Reports;

namespace Infrastructure.Common.Windows.Reports
{
	public interface ISingleReportProvider : IReportProvider
	{
		ReportData GetData();
	}
}
