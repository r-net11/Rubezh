using System.Collections.Generic;

namespace FiresecAPI.SKD.ReportFilters
{
	public interface IReportFilterSchedule
	{
		List<int> Schedules { get; set; }

		bool ScheduleEnter { get; set; }

		bool ScheduleExit { get; set; }
	}
}