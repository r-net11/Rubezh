using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.SKD.ReportFilters;

namespace Infrastructure.Common.SKDReports
{
	public class FilterModel
	{
		public FilterModel()
		{
			AllowSort = true;
			HasPeriod = false;
		}

		public bool HasPeriod { get; set; }
		public bool AllowSort { get; set; }
		public Dictionary<string, string> Columns { get; set; }
		public FilterContainerViewModel CommandsViewModel { get; set; }
		public FilterContainerViewModel MainViewModel { get; set; }
		public IEnumerable<FilterContainerViewModel> Pages { get; set; }
	}
}
