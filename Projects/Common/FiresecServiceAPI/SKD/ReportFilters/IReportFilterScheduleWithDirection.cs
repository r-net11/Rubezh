
namespace FiresecAPI.SKD.ReportFilters
{
	public interface IReportFilterScheduleWithDirection : IReportFilterSchedule
	{
		bool ScheduleEnter { get; set; }
		bool ScheduleExit { get; set; }
	}
}