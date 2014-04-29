using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;
using Schedule = FiresecAPI.EmployeeTimeIntervals.Schedule;

namespace SKDModule.ViewModels
{
	public class SelectScheduleViewModel : SaveCancelDialogViewModel
	{
		public Employee Employee { get; private set; }

		public SelectScheduleViewModel(Employee employee)
		{
			Title = "График работы";
			Employee = employee;
			Schedules = new List<SelectationScheduleViewModel>();
			var schedules = ScheduleHelper.GetByOrganisation(Employee.OrganisationUID);
			if (schedules == null)
				return;
			StartDate = employee.ScheduleStartDate;
			foreach (var schedule in schedules)
				Schedules.Add(new SelectationScheduleViewModel(schedule));
			if (Schedules.Count > 0)
			{
				if (Employee.ScheduleUID != null)
				{
					var selectedSchedule = Schedules.FirstOrDefault(x => x.Schedule.UID == Employee.ScheduleUID);
					if (selectedSchedule != null)
						selectedSchedule.IsChecked = true;
					else
						Schedules.FirstOrDefault().IsChecked = true;
				}
				else
					Schedules.FirstOrDefault().IsChecked = true;
			}
		}
		
		DateTime _startDate;
		public DateTime StartDate
		{
			get { return _startDate; }
			set
			{
				_startDate = value;
				OnPropertyChanged(() => StartDate);
			}
		}

		public List<SelectationScheduleViewModel> Schedules { get; private set; }
		public SelectationScheduleViewModel SelectedSchedule
		{
			get { return Schedules.FirstOrDefault(x => x.IsChecked); }
		}
	}

	public class SelectationScheduleViewModel : BaseViewModel
	{
		public Schedule Schedule { get; private set; }

		public SelectationScheduleViewModel(Schedule schedule)
		{
			Schedule = schedule;
		}

		public string Name { get { return Schedule.Name; } }
		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");
			}
		}
	}
}
