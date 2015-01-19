using System;
using System.Collections.Generic;

namespace FiresecAPI.SKD.ReportFilters
{
	public interface IReportFilterSchedule
	{
		List<Guid> Schedules { get; set; }
	}
}
