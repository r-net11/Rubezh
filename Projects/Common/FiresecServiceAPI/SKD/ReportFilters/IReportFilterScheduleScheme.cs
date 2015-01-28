
using System.Collections.Generic;
using System;
namespace FiresecAPI.SKD.ReportFilters
{
	public interface IReportFilterScheduleScheme
	{
		List<Guid> ScheduleSchemas { get; set; }
	}
}