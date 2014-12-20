using System;
using System.Collections.Generic;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.SKDReports
{
	public interface IFilteredSKDReportProvider : ISKDReportProvider
	{
		Type FilterType { get; }
		SKDReportFilter FilterObject { get; }

		FilterModel CreateFilterModel();
		void UpdateFilter(SKDReportFilter filter);
	}
}
