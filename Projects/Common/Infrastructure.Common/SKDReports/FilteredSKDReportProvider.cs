using System;

namespace Infrastructure.Common.SKDReports
{
	public class FilteredSKDReportProvider<T> : SKDReportProvider, IFilteredSKDReportProvider
	{
		public T Filter { get; set; }
		public Predicate<T> EditFilter { get; set; }

		public FilteredSKDReportProvider(string name, string title, int index, SKDReportGroup? group = null, Predicate<T> editFilter = null, T filter = default(T))
			: base(name, title, index, group)
		{
			EditFilter = editFilter;
			Filter = filter;
		}

		#region IFilteredSKDReportProvider Members

		public Type FilterType
		{
			get { return typeof(T); }
		}

		public object FilterObject
		{
			get { return Filter; }
		}

		public virtual bool ChangeFilter()
		{
			if (EditFilter == null)
				return false;
			return EditFilter(Filter);
		}

		#endregion
	}

}
