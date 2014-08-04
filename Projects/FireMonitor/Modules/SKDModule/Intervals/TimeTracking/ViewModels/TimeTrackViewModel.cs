using FiresecAPI.SKD.EmployeeTimeIntervals;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using FiresecAPI.SKD;

namespace SKDModule.ViewModels
{
	public class TimeTrackViewModel : BaseObjectViewModel<TimeTrack>
	{
		public TimeTrack TimeTrack { get; private set; }

		public TimeTrackViewModel(TimeTrack timeTrack)
			: base(timeTrack)
		{
			TimeTrack = timeTrack;
			DayTracks = new ObservableCollection<DayTrackViewModel>();
			foreach (var employeeTimeTrack in timeTrack.EmployeeTimeTracks)
			{
				var dayTrackViewModel = new DayTrackViewModel(employeeTimeTrack);
				DayTracks.Add(dayTrackViewModel);
			}
		}

		public string FIO
		{
			get
			{
				return TimeTrack.LastName + " " + TimeTrack.FirstName + " " + TimeTrack.SecondName;
			}
		}

		public ObservableCollection<DayTrackViewModel> DayTracks { get; set; }
	}
}