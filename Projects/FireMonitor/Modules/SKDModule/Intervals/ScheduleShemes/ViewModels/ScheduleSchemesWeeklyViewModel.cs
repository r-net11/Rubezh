using FiresecAPI.EmployeeTimeIntervals;

namespace SKDModule.ViewModels
{
	public class ScheduleSchemesWeeklyViewModel : ScheduleSchemesViewModel
	{
		public ScheduleSchemesWeeklyViewModel()
			: base()
		{
		}
		public override ScheduleSchemeType Type
		{
			get { return ScheduleSchemeType.Week; }
		}
	}
}