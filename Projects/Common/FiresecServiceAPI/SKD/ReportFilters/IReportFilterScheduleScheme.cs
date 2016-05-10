using System;
using System.Collections.Generic;

namespace StrazhAPI.SKD.ReportFilters
{
	public interface IReportFilterScheduleScheme
	{
		List<Guid> ScheduleSchemas { get; set; }
	}
}