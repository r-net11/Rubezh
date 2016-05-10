using System;
using System.Collections.Generic;
using RubezhAPI.SKD.ReportFilters;

namespace FiresecService.Report.Reports
{
	/// <summary>
	/// Base Class for Reports.
	/// </summary>
	public abstract class BaseReport<TData>
	{
		public abstract TData CreateDataSet(DataProvider dataProvider, SKDReportFilter filter);

		public TData CreateDataSet(DataProvider dataProvider)
		{
			return this.CreateDataSet(dataProvider, null);
		}

		protected TFilter GetFilter<TFilter>(SKDReportFilter filter)
			where TFilter : SKDReportFilter
		{
			return (TFilter)filter ?? Activator.CreateInstance<TFilter>();
		}

		protected void ThrowException(string message)
		{
			throw new Exception(message);
		}

	}
}
