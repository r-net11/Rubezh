using FiresecAPI.EmployeeTimeIntervals;

namespace SKDModule.ViewModels
{
	public class ScheduleSchemesSlideViewModel : ScheduleSchemesViewModel
	{
		public ScheduleSchemesSlideViewModel()
			: base()
		{
		}
		public override ScheduleSchemeType Type
		{
			get { return ScheduleSchemeType.SlideDay; }
		}
	}
}