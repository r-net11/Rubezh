using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.SKDReports;
using FiresecAPI.SKD.ReportFilters;

namespace ReportsModule.ViewModels
{
	public class FilterSortViewModel : FilterContainerViewModel
	{
		public FilterSortViewModel(Dictionary<string, string> columns)
		{
			Title = "Сортировка";
			Columns = columns;
		}

		public Dictionary<string, string> Columns { get; private set; }
		private bool _sortAscending;
		public bool SortAscending
		{
			get { return _sortAscending; }
			set
			{
				_sortAscending = value;
				OnPropertyChanged(() => SortAscending);
			}
		}
		

		public override void LoadFilter(SKDReportFilter filter)
		{
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
		}
	}
}
