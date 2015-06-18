using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;
using Schedule = FiresecAPI.SKD.Schedule;

namespace SKDModule.ViewModels
{
	public class ScheduleSelectionViewModel : SaveCancelDialogViewModel
	{
		public Employee Employee { get; private set; }

		public ScheduleSelectionViewModel(Employee employee, ShortSchedule shortSchedule, DateTime startDate)
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
			if (shortSchedule != null)
			{
				SelectedSchedule = Schedules.FirstOrDefault(x => x.UID == shortSchedule.UID);
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

		public ShortSchedule ShortSchedule
		{
			get
			{
				return new ShortSchedule
					{
						UID = SelectedSchedule.UID,
						Description = SelectedSchedule.Description,
						IsDeleted = SelectedSchedule.IsDeleted,
						Name = SelectedSchedule.Name,
						OrganisationUID = SelectedSchedule.OrganisationUID,
						RemovalDate = SelectedSchedule.RemovalDate
					};
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