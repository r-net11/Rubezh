using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Schedule = FiresecAPI.EmployeeTimeIntervals.Schedule;

namespace SKDModule.ViewModels
{
	public class SelectScheduleViewModel : SaveCancelDialogViewModel
	{
		public Employee Employee { get; private set; }

		public SelectScheduleViewModel(Employee employee, ShortSchedule schedule, DateTime startDate)
		{
			Title = "Должность";
			Employee = employee;
			StartDate = startDate;
			Schedules = new ObservableCollection<SelectationScheduleViewModel>();
			var schedules = ScheduleHelper.GetShortByOrganisation(Employee.OrganisationUID);
			if (schedules == null || schedules.Count() == 0)
			{
				MessageBoxService.Show("Для данной организации не указано не одного графика работы");
				return;
			}
			foreach (var item in schedules)
				Schedules.Add(new SelectationScheduleViewModel(item, this));
			SelectationScheduleViewModel selectedSchedule;
			if (schedule != null)
			{
				selectedSchedule = Schedules.FirstOrDefault(x => x.Schedule.UID == schedule.UID);
				if (selectedSchedule == null)
					selectedSchedule = Schedules.FirstOrDefault();
			}
			else
				selectedSchedule = Schedules.FirstOrDefault();
			selectedSchedule.IsChecked = true;
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

		public ObservableCollection<SelectationScheduleViewModel> Schedules { get; set; }

		public SelectationScheduleViewModel SelectedSchedule
		{
			get { return Schedules.FirstOrDefault(x => x.IsChecked); }
		}
	}

	public class SelectationScheduleViewModel : BaseViewModel
	{
		public ShortSchedule Schedule { get; private set; }
		SelectScheduleViewModel SelectScheduleViewModel;

		public SelectationScheduleViewModel(ShortSchedule schedule, SelectScheduleViewModel selectScheduleViewModel)
		{
			Schedule = schedule;
			SelectScheduleViewModel = selectScheduleViewModel;
			SelectCommand = new RelayCommand(OnAdd);
		}

		public string Name { get { return Schedule.Name; } }
		
		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
			}
		}

		public RelayCommand SelectCommand { get; private set; }
		void OnAdd()
		{
			var Schedule = SelectScheduleViewModel.Schedules.FirstOrDefault(x => x.IsChecked);
			if (Schedule != null)
				Schedule.IsChecked = false;
			IsChecked = true;
		}
	}
}
