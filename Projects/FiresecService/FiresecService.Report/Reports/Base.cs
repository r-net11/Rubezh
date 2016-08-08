using StrazhAPI.SKD;
using StrazhAPI.SKD.ReportFilters;
using System;

namespace FiresecService.Report.Reports
{
	public abstract class Base<T>
	{
	//	public abstract T CreateDataSet(DataProvider dataProvider, SKDReportFilter filter);
		public abstract T CreateDataSet(DataProvider dataProvider, OrganisationFilterBase filter);

		public T CreateDataSet(DataProvider provider)
		{
			return CreateDataSet(provider, null);
		}

		protected TFilter GetFilter<TFilter>(OrganisationFilterBase filter) where TFilter : OrganisationFilterBase
		{
			return (TFilter)filter ?? Activator.CreateInstance<TFilter>();
		}

		//protected TFilter GetFilter<TFilter>(SKDReportFilter filter) where TFilter : SKDReportFilter
		//{
		//	return (TFilter) filter ?? Activator.CreateInstance<TFilter>();
		//}
	}
}
