
using System;
using System.Collections.Generic;
namespace RubezhAPI.SKD.ReportFilters
{
	public interface IReportFilterScheduleScheme
	{
		List<Guid> ScheduleSchemas { get; set; }
	}
}