using System.Linq;
using FiresecAPI.Automation;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class ScheduleViewModel : BaseViewModel
	{
		public const string DefaultName = "<нет>";
		public AutomationSchedule Schedule { get; set; }
		public ObservableCollection<ProcedureViewModel> Procedures { get; private set; }
		
		public ScheduleViewModel(AutomationSchedule schedule)
		{
			Schedule = schedule;
			Procedures = new ObservableCollection<ProcedureViewModel>();
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanDeleted);
			InitializeProcedure();
		}

		void InitializeProcedure()
		{
			foreach (var procedure in FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures)
			{
				if (!Schedule.ProceduresUids.Contains(procedure.Uid))
					continue;
				var procedureViewModel = new ProcedureViewModel(procedure);
				Procedures.Add(procedureViewModel);
			}
			SelectedProcedure = Procedures.FirstOrDefault();
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
				OnPropertyChanged("Name");
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

		bool _isEnabled;
		public bool IsEnabled
		{
			get { return _isEnabled; }
			set
			{
				_isEnabled = value;
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

		private ProcedureViewModel _selectedProcedure;
		public ProcedureViewModel SelectedProcedure
		{
			get { return _selectedProcedure; }
			set
			{
				_selectedProcedure = value;
				OnPropertyChanged(() => SelectedProcedure);
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
			var procedureSelectionViewModel = new ProcedureSelectionViewModel(Procedures);
			if (DialogService.ShowModalWindow(procedureSelectionViewModel))
			{
				if (procedureSelectionViewModel.SelectedProcedure != null)
				{
					var procedureViewModel = new ProcedureViewModel(procedureSelectionViewModel.SelectedProcedure.Procedure);
					Procedures.Add(procedureViewModel);
					Schedule.ProceduresUids.Add(procedureViewModel.Procedure.Uid);
					SelectedProcedure = procedureViewModel;
					ServiceFactory.SaveService.AutomationChanged = true;
				}
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			Schedule.ProceduresUids.Remove(SelectedProcedure.Procedure.Uid);
			Procedures.Remove(SelectedProcedure);			
			SelectedProcedure = Procedures.FirstOrDefault();
			ServiceFactory.SaveService.AutomationChanged = true;
		}
		bool CanDeleted()
		{
			return SelectedProcedure != null;
		}
	}
}