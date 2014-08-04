using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.SKD;
using System.Collections.ObjectModel;

namespace SKDModule.ViewModels
{
	public class TimeTrackDetailsViewModel : SaveCancelDialogViewModel
	{
		EmployeeTimeTrack EmployeeTimeTrack;

		public TimeTrackDetailsViewModel(EmployeeTimeTrack employeeTimeTrack)
		{
			Title = "Время в течение дня";
			EmployeeTimeTrack = employeeTimeTrack;

			EmployeeTimeTrackParts = new ObservableCollection<EmployeeTimeTrackPartViewModel>();
			foreach (var employeeTimeTrackPart in EmployeeTimeTrack.EmployeeTimeTrackParts)
			{
				var employeeTimeTrackPartViewModel = new EmployeeTimeTrackPartViewModel(employeeTimeTrackPart);
				EmployeeTimeTrackParts.Add(employeeTimeTrackPartViewModel);
			}
		}

		public ObservableCollection<EmployeeTimeTrackPartViewModel> EmployeeTimeTrackParts { get; private set; }
	}

	public class EmployeeTimeTrackPartViewModel : BaseViewModel
	{
		public EmployeeTimeTrackPart EmployeeTimeTrackPart { get; private set; }

		public EmployeeTimeTrackPartViewModel(EmployeeTimeTrackPart employeeTimeTrackPart)
		{
			EmployeeTimeTrackPart = employeeTimeTrackPart;
		}
	}
}