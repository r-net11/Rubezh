using System;
using System.Collections.Generic;

namespace StrazhAPI.SKD.ReportFilters
{
	public interface IReportFilterEmployee
	{
		List<Guid> Employees { get; set; }

		bool IsSearch { get; set; }

		string LastName { get; set; }

		string FirstName { get; set; }

		string SecondName { get; set; }
	}
}