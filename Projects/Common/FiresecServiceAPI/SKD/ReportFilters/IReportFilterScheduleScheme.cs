using System;
using System.Collections.Generic;

namespace FiresecAPI.SKD.ReportFilters
{
	public interface IReportFilterScheduleScheme
	{
		List<Guid> ScheduleSchemas { get; set; }
	}
}