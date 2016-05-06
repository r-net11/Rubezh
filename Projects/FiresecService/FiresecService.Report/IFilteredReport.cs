using StrazhAPI.SKD.ReportFilters;

namespace FiresecService.Report
{
	public interface IFilteredReport
	{
		void ApplyFilter(SKDReportFilter filter);
	}
}