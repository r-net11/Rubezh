using System;
using System.Data;
using RubezhAPI.SKD.ReportFilters;

namespace FiresecService.Report.Reports
{
	/// <summary>
	/// Base Class for Reports.
	/// </summary>
	public abstract class BaseReport
	{
		public abstract DataSet CreateDataSet(DataProvider dataProvider, SKDReportFilter filter);

		public DataSet CreateDataSet(DataProvider dataProvider)
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
