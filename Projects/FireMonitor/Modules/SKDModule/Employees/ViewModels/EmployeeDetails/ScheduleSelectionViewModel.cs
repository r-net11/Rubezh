using System;
using System.Collections.ObjectModel;
using System.Linq;
using StrazhAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Schedule = StrazhAPI.SKD.Schedule;

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

			Schedules = new ObservableCollection<ShortSchedule>();
			var schedules = ScheduleHelper.GetShortByOrganisation(Employee.OrganisationUID);
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

		public ObservableCollection<ShortSchedule> Schedules { get; set; }

		ShortSchedule _selectedSchedule;
		public ShortSchedule SelectedSchedule
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