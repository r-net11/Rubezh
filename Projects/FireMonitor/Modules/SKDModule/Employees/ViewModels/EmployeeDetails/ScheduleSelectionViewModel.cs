using System;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Schedule = RubezhAPI.SKD.Schedule;

namespace SKDModule.ViewModels
{
	public class ScheduleSelectionViewModel : SaveCancelDialogViewModel
	{
		public Employee Employee { get; private set; }

		public ScheduleSelectionViewModel(Employee employee, EmployeeItem employeeSchedule, DateTime startDate)
		{
			Title = "Выбор графика работы";
			Employee = employee;
			StartDate = startDate;

			Schedules = new ObservableCollection<Schedule>();
			var schedules = ScheduleHelper.GetByOrganisation(Employee.OrganisationUID);
			if (schedules != null)
			{
				foreach (var schedule in schedules)
				{
					Schedules.Add(schedule);
				}
			}
			if (employeeSchedule != null)
			{
				SelectedSchedule = Schedules.FirstOrDefault(x => x.UID == employeeSchedule.UID);
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

		public ObservableCollection<Schedule> Schedules { get; set; }

		Schedule _selectedSchedule;
		public Schedule SelectedSchedule
		{
			get { return _selectedSchedule; }
			set
			{
				_selectedSchedule = value;
				OnPropertyChanged(() => SelectedSchedule);
			}
		}
	}
}