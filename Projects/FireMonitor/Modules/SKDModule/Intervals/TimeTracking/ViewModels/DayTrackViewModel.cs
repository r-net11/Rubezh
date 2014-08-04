using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.SKD;

namespace SKDModule.ViewModels
{
	public class DayTrackViewModel : BaseViewModel
	{
		public EmployeeTimeTrack EmployeeTimeTrack { get; private set; }

		public DayTrackViewModel(EmployeeTimeTrack employeeTimeTrack)
		{
			EmployeeTimeTrack = employeeTimeTrack;
			Total = DateTimeToString(EmployeeTimeTrack.Total);
			TotalMiss = DateTimeToString(EmployeeTimeTrack.TotalMiss);
			TotalInSchedule = DateTimeToString(EmployeeTimeTrack.TotalInSchedule);
			TotalOutSchedule = DateTimeToString(EmployeeTimeTrack.TotalOutSchedule);
		}

		public string Total { get; private set; }
		public string TotalMiss { get; private set; }
		public string TotalInSchedule { get; private set; }
		public string TotalOutSchedule { get; private set; }

		public bool ShowTotal { get; private set; }

		public string DateTimeToString(DateTime dateTime)
		{
			if(EmployeeTimeTrack.Total.Hour > 0)
			return EmployeeTimeTrack.Total.Hour + "ч " + EmployeeTimeTrack.Total.Minute + "м";
			else
				return EmployeeTimeTrack.Total.Minute + "м";
		}
	}
}