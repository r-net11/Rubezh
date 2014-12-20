using System;
using FiresecAPI.SKD.ReportFilters;

namespace Infrastructure.Common.SKDReports
{
	public abstract class FilteredSKDReportProvider<T> : SKDReportProvider, IFilteredSKDReportProvider
		where T : SKDReportFilter
	{
		public FilteredSKDReportProvider(string name, string title, int index, SKDReportGroup? group = null)
			: base(name, title, index, group)
		{
			Filter = Activator.CreateInstance<T>();
		}

		protected T Filter { get; private set; }

		#region IFilteredSKDReportProvider Members

		public Type FilterType
		{
			get { return typeof(T); }
		}
		public SKDReportFilter FilterObject
		{
			get { return Filter; }
		}

		public abstract FilterModel CreateFilterModel();

		public void UpdateFilter(SKDReportFilter filter)
		{
			Filter = filter as T;
		}

		#endregion
	}
}
