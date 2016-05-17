using RubezhAPI.SKD.ReportFilters;

namespace RubezhService.Report
{
	public interface IFilteredReport
	{
		void ApplyFilter(SKDReportFilter filter);
	}
}
