using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.SKDReports;
using StrazhAPI.SKD.ReportFilters;

namespace ReportsModule.ViewModels
{
	public class FilterSortPageViewModel : FilterContainerViewModel
	{
		public FilterSortPageViewModel(Dictionary<string, string> columns)
		{
			Title = "Сортировка";
			Columns = columns;
			if (Columns.Any())
				SortColumn = Columns.First().Key;
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
		private string _sortColumn;
		public string SortColumn
		{
			get { return _sortColumn; }
			set
			{
				_sortColumn = value;
				OnPropertyChanged(() => SortColumn);
			}
		}

		public override void LoadFilter(SKDReportFilter filter)
		{
			if (Columns.Any())
				SortColumn = !string.IsNullOrEmpty(filter.SortColumn) && Columns.ContainsKey(filter.SortColumn) ? filter.SortColumn : Columns.First().Key;
			SortAscending = filter.SortAscending;
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			filter.SortColumn = SortColumn;
			filter.SortAscending = SortAscending;
		}
	}
}
