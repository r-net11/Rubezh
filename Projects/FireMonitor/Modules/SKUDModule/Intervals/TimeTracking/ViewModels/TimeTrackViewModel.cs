using FiresecAPI.SKD.EmployeeTimeIntervals;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class TimeTrackViewModel : BaseObjectViewModel<TimeTrack>
	{
		public TimeTrackViewModel(TimeTrack timeTrack)
			: base(timeTrack)
		{
		}
	}
}