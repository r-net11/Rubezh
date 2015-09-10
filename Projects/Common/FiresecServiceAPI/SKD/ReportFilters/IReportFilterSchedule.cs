using System.Collections.Generic;

namespace FiresecAPI.SKD.ReportFilters
{
	public interface IReportFilterSchedule
	{
		List<int> Schedules { get; set; }
	}
}