using System.Collections.Generic;

namespace StrazhAPI.SKD.ReportFilters
{
	public interface IReportFilterSchedule
	{
		List<int> Schedules { get; set; }
	}
}