using System;
using System.Collections.Generic;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.Windows.ViewModels;

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
