using System;
using System.Collections.Generic;

namespace FiresecAPI.SKD.ReportFilters
{
	public interface IReportFilterEmployee
	{
		List<Guid> Employees { get; set; }
	}
}
