using FiresecAPI.EmployeeTimeIntervals;

namespace SKDModule.ViewModels
{
	public class ScheduleSchemesMonthlyViewModel : ScheduleSchemesViewModel
	{
		public ScheduleSchemesMonthlyViewModel()
			: base()
		{
		}
		public override ScheduleSchemeType Type
		{
			get { return ScheduleSchemeType.Month; }
		}
	}
}