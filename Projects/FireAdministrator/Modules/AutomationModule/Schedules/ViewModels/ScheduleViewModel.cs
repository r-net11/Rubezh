using System.Collections.Generic;
using System.Linq;
using StrazhAPI.Automation;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.ObjectModel;
using Localization.Automation;

namespace AutomationModule.ViewModels
{
	public class ScheduleViewModel : BaseViewModel
	{
        public static string DefaultName = CommonResources.NO_DefaultName;
		public AutomationSchedule Schedule { get; set; }
		public ObservableCollection<ScheduleProcedureViewModel> ScheduleProcedures { get; private set; }
		
		public ScheduleViewModel(AutomationSchedule schedule)
		{
			Schedule = schedule;
			ScheduleProcedures = new ObservableCollection<ScheduleProcedureViewModel>();
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanDeleted);
			UpdateContent();
		}

		public void UpdateContent()
		{
			ScheduleProcedures = new ObservableCollection<ScheduleProcedureViewModel>();
			foreach (var scheduleProcedure in Schedule.ScheduleProcedures)
			{
				var procedure = FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures.FirstOrDefault(x => x.Uid == scheduleProcedure.ProcedureUid);
				if (procedure != null)
				{
					var scheduleProcedureViewModel = new ScheduleProcedureViewModel(scheduleProcedure);
					ScheduleProcedures.Add(scheduleProcedureViewModel);
				}
			}
			SelectedScheduleProcedure = ScheduleProcedures.FirstOrDefault();
			OnPropertyChanged(() => SelectedScheduleProcedure);
			OnPropertyChanged(() => ScheduleProcedures);
		}

		static ScheduleViewModel()
		{
			Years = new ObservableCollection<int> { -1 };
			for (int i = DateTime.Now.Year; i < DateTime.Now.Year + 100; i++)
			{
				Years.Add(i);
			}
			Months = new ObservableCollection<int> { -1 };
			for (int i = 1; i <= 12; i++)
			{
				Months.Add(i);				
			}
			Days = new ObservableCollection<int> { -1 };
			PeriodDays = new ObservableCollection<int> { 0 };
			for (int i = 1; i <= 31 ; i++)
			{
				Days.Add(i);
				PeriodDays.Add(i);
			}
			Hours = new ObservableCollection<int> { -1 };
			PeriodHours = new ObservableCollection<int>();
			for (int i = 0; i <= 24; i++)
			{
				Hours.Add(i);
				PeriodHours.Add(i);
			}
			Minutes = new ObservableCollection<int> { -1 };
			PeriodMinutes = new ObservableCollection<int>();
			for (int i = 0; i <= 59; i++)
			{
				Minutes.Add(i);
				PeriodMinutes.Add(i);
			}
			Seconds = new ObservableCollection<int> { -1 };
			PeriodSeconds = new ObservableCollection<int>();
			for (int i = 0; i <= 59; i++)
			{
				Seconds.Add(i);
				PeriodSeconds.Add(i);
			}
			DaysOfWeek = new ObservableCollection<DayOfWeekType>
				{
					DayOfWeekType.Any, DayOfWeekType.Monday, DayOfWeekType.Tuesday, DayOfWeekType.Wednesday,
					DayOfWeekType.Thursday, DayOfWeekType.Friday, DayOfWeekType.Saturday, DayOfWeekType.Sunday
				};			
		}

		public string Name
		{
			get { return Schedule.Name; }
			set
			{
				Schedule.Name = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => Name);
			}
		}

		public static ObservableCollection<int> Years { get; private set; }
		public static ObservableCollection<int> Months { get; private set; }
		public static ObservableCollection<int> Days { get; private set; }
		public static ObservableCollection<int> Hours { get; private set; }
		public static ObservableCollection<int> Minutes { get; private set; }
		public static ObservableCollection<int> Seconds { get; private set; }
		public static ObservableCollection<int> PeriodDays { get; private set; }
		public static ObservableCollection<int> PeriodHours { get; private set; }
		public static ObservableCollection<int> PeriodMinutes { get; private set; }
		public static ObservableCollection<int> PeriodSeconds { get; private set; }
		public static ObservableCollection<DayOfWeekType> DaysOfWeek { get; private set; }

		public bool IsEnabled
		{
			get { return Schedule.IsActive; }
			set
			{
				Schedule.IsActive = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => IsEnabled);
			}
		}

		public int SelectedYear
		{
			get { return Schedule.Year; }
			set
			{
				Schedule.Year = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedYear);
			}
		}

		public int SelectedMonth
		{
			get { return Schedule.Month; }
			set
			{
				Schedule.Month = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedMonth);
			}
		}

		public int SelectedDay
		{
			get { return Schedule.Day; }
			set
			{
				Schedule.Day = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedDay);
			}
		}

		public int SelectedHour
		{
			get { return Schedule.Hour; }
			set
			{
				Schedule.Hour = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedHour);
			}
		}

		public int SelectedMinute
		{
			get { return Schedule.Minute; }
			set
			{
				Schedule.Minute = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedMinute);
			}
		}

		public int SelectedSecond
		{
			get { return Schedule.Second; }
			set
			{
				Schedule.Second = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedSecond);
			}
		}

		public DayOfWeekType SelectedDayOfWeek
		{
			get { return Schedule.DayOfWeek; }
			set
			{
				Schedule.DayOfWeek = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedDayOfWeek);
			}
		}

		public int SelectedPeriodDay
		{
			get { return Schedule.PeriodDay; }
			set
			{
				Schedule.PeriodDay = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedPeriodDay);
			}
		}

		public int SelectedPeriodHour
		{
			get { return Schedule.PeriodHour; }
			set
			{
				Schedule.PeriodHour = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedPeriodHour);
			}
		}

		public int SelectedPeriodMinute
		{
			get { return Schedule.PeriodMinute; }
			set
			{
				Schedule.PeriodMinute = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedPeriodMinute);
			}
		}

		public int SelectedPeriodSecond
		{
			get { return Schedule.PeriodSecond; }
			set
			{
				Schedule.PeriodSecond = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedPeriodSecond);
			}
		}

		private ScheduleProcedureViewModel _selectedScheduleProcedure;
		public ScheduleProcedureViewModel SelectedScheduleProcedure
		{
			get { return _selectedScheduleProcedure; }
			set
			{
				_selectedScheduleProcedure = value;
				OnPropertyChanged(() => SelectedScheduleProcedure);
			}
		}

		public bool IsPeriodSelected
		{
			get { return Schedule.IsPeriodSelected; }
			set
			{
				Schedule.IsPeriodSelected = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => IsPeriodSelected);
			}
		}

		public void Update()
		{
			OnPropertyChanged(() => Schedule);
			OnPropertyChanged(() => Name);
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var procedureSelectionViewModel = new ProcedureSelectionViewModel();
			if (DialogService.ShowModalWindow(procedureSelectionViewModel))
			{
				if (procedureSelectionViewModel.SelectedProcedure != null)
				{
					var scheduleProcedure = new ScheduleProcedure();
					scheduleProcedure.ProcedureUid = procedureSelectionViewModel.SelectedProcedure.Procedure.Uid;					
					var scheduleProcedureViewModel = new ScheduleProcedureViewModel(scheduleProcedure);
					ScheduleProcedures.Add(scheduleProcedureViewModel);
					Schedule.ScheduleProcedures.Add(scheduleProcedureViewModel.ScheduleProcedure);
					SelectedScheduleProcedure = scheduleProcedureViewModel;
					ServiceFactory.SaveService.AutomationChanged = true;
				}
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var index = ScheduleProcedures.IndexOf(SelectedScheduleProcedure);
			Schedule.ScheduleProcedures.Remove(SelectedScheduleProcedure.ScheduleProcedure);
			ScheduleProcedures.Remove(SelectedScheduleProcedure);
			index = Math.Min(index, ScheduleProcedures.Count - 1);
			if (index > -1)
				SelectedScheduleProcedure = ScheduleProcedures[index];
			ServiceFactory.SaveService.AutomationChanged = true;
		}
		bool CanDeleted()
		{
			return SelectedScheduleProcedure != null;
		}
	}
}