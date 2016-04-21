using System;
using RubezhAPI.SKD.ReportFilters;

namespace Infrastructure.Common.SKDReports
{
	public interface IFilteredSKDReportProvider : ISKDReportProvider
	{
		Type FilterType { get; }

		SKDReportFilter GetFilter();
		FilterModel GetFilterModel();
		void UpdateFilter(SKDReportFilter filter);
	}
}