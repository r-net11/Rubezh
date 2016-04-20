using System.Collections.Generic;

namespace Infrastructure.Common.SKDReports
{
	public class FilterModel
	{
		public FilterModel()
		{
			AllowSort = true;
		}

		public bool AllowSort { get; set; }
		public Dictionary<string, string> Columns { get; set; }
		public FilterContainerViewModel CommandsViewModel { get; set; }
		public FilterContainerViewModel MainViewModel { get; set; }
		public IEnumerable<FilterContainerViewModel> Pages { get; set; }
	}
}