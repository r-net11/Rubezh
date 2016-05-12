using System;
using RubezhAPI.Models;
using RubezhAPI.SKD.ReportFilters;
using Infrastructure.Common.Windows;

namespace Infrastructure.Common.SKDReports
{
	public abstract class FilteredSKDReportProvider<T> : SKDReportProvider, IFilteredSKDReportProvider
		where T : SKDReportFilter
	{
		public FilteredSKDReportProvider(string title, int index, SKDReportGroup? group = null, PermissionType? permission = null)
			: base(title, index, group, permission)
		{
			Filter = Activator.CreateInstance<T>();
			FilterModel = InitializeFilterModel();
		}

		protected T Filter { get; private set; }
		protected FilterModel FilterModel { get; set; }
		public abstract FilterModel InitializeFilterModel();

		#region IFilteredSKDReportProvider Members

		public Type FilterType
		{
			get { return typeof(T); }
		}

		public virtual SKDReportFilter GetFilter()
		{
			Filter.User = ApplicationService.User;
			Filter.Timestamp = DateTime.Now;
			return Filter;
		}

		public FilterModel GetFilterModel() {return  FilterModel;}

		public void UpdateFilter(SKDReportFilter filter)
		{
			Filter = filter as T;
		}

		#endregion
	}
}