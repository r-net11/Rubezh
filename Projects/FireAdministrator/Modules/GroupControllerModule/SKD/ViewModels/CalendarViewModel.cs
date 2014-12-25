using System.Globalization;
using System.Linq;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using Calendar = FiresecAPI.GK.Calendar;

namespace GKModule.ViewModels
{
	public class CalendarViewModel : BaseViewModel
	{
		public ObservableCollection<MonthViewModel> Months{get;private set;}
		Calendar Calendar { get; set; }
		public MonthViewModel JanuaryMonth { get; private set; }
		public MonthViewModel FebruaryMonth { get; private set; }
		public MonthViewModel MarchMonth { get; private set; }
		public MonthViewModel AprilMonth { get; private set; }
		public MonthViewModel MayMonth { get; private set; }
		public MonthViewModel JuneMonth { get; private set; }
		public MonthViewModel JulyMonth { get; private set; }
		public MonthViewModel AugustMonth { get; private set; }
		public MonthViewModel SeptemberMonth { get; private set; }
		public MonthViewModel OctoberMonth { get; private set; }
		public MonthViewModel NovemberMonth { get; private set; }
		public MonthViewModel DecemberMonth { get; private set; }

		public CalendarViewModel(Calendar calendar)
		{
			Calendar = calendar;
			Months = new ObservableCollection<MonthViewModel>();
			Months.Add(JanuaryMonth = new MonthViewModel(MonthType.January, Calendar));
			Months.Add(FebruaryMonth = new MonthViewModel(MonthType.February, Calendar));
			Months.Add(MarchMonth = new MonthViewModel(MonthType.March, Calendar));
			Months.Add(AprilMonth = new MonthViewModel(MonthType.April, Calendar));
			Months.Add(MayMonth = new MonthViewModel(MonthType.May, Calendar));
			Months.Add(JuneMonth = new MonthViewModel(MonthType.June, Calendar));
			Months.Add(JulyMonth = new MonthViewModel(MonthType.July, Calendar));
			Months.Add(AugustMonth = new MonthViewModel(MonthType.August, Calendar));
			Months.Add(SeptemberMonth = new MonthViewModel(MonthType.September, Calendar));
			Months.Add(OctoberMonth = new MonthViewModel(MonthType.October, Calendar));
			Months.Add(NovemberMonth = new MonthViewModel(MonthType.November, Calendar));
			Months.Add(DecemberMonth = new MonthViewModel(MonthType.December, Calendar));
			PreviousYearCommand = new RelayCommand(OnPreviousYear);
			NextYearCommand = new RelayCommand(OnNextYear);
			UpdateMonths();
		}

		public RelayCommand PreviousYearCommand { get; private set; }
		void OnPreviousYear()
		{
			Year--;
		}

		public RelayCommand NextYearCommand { get; private set; }
		void OnNextYear()
		{
			Year++;
		}

		public int Year
		{
			get { return Calendar.Year; }
			set
			{
				Calendar.Year = value;
				UpdateMonths();
				OnPropertyChanged(() => Year);
			}
		}

		void UpdateMonths()
		{
			foreach (var month in Months)
			{
				month.Update(Year);
			}
			foreach (var selectedDay in Calendar.SelectedDays.FindAll(x => x.Year == Year))
			{
				var month = Months.FirstOrDefault(x => (int) x.MonthType == selectedDay.Month);
				if (month != null)
				{
					var day = month.Days.FirstOrDefault(x => x.No == selectedDay.Day);
					if (day != null)
						day.IsSelected = true;
				}
			}
			OnPropertyChanged(() => Months);
		}
	}

	public class MonthViewModel : BaseViewModel
	{
		public MonthType MonthType { get; private set; }
		public List<DayViewModel> Days { get; private set; }
		public Calendar Calendar { get; private set; }

		public MonthViewModel(MonthType monthType, Calendar calendar)
		{
			Calendar = calendar;
			MonthType = monthType;
			Update(Calendar.Year);
		}

		public void Update(int year)
		{
			Days = new List<DayViewModel>();
			for (int i = 1; i <= 31; i++)
			{
				if ((MonthType == MonthType.April || MonthType == MonthType.June || MonthType == MonthType.September || MonthType == MonthType.November)&&(i == 31))
					break;
				if (MonthType == MonthType.February && year % 4 == 0 && i == 30)
					break;
				if (MonthType == MonthType.February && year % 4 != 0 && i == 29)
					break;
				Days.Add(new DayViewModel(i, MonthType, Calendar));
			}
			OnPropertyChanged(()=>Days);
		}
	}

	public class DayViewModel : BaseViewModel
	{
		public int No { get; private set; }
		public MonthType MonthType { get; private set; }
		public Calendar Calendar { get; private set; } 
		public DayOfWeek DayOfWeek { get; private set; }

		public DayViewModel(int no, MonthType monthType, Calendar calendar)
		{
			Calendar = calendar;
			var dateTime = new DateTime(Calendar.Year, (int)monthType, no);
			DayOfWeek = dateTime.DayOfWeek;
			MonthType = monthType;
			No = no;
			SelectCommand = new RelayCommand(OnSelect);
		}

		public RelayCommand SelectCommand { get; private set; }
		void OnSelect()
		{
			IsSelected = !IsSelected;
			var dateTime = new DateTime(Calendar.Year, (int)MonthType, No);
			if (IsSelected)
				Calendar.SelectedDays.Add(dateTime);
			else
				Calendar.SelectedDays.Remove(dateTime);
			ServiceFactory.SaveService.GKChanged = true;
		}

		public int X
		{
			get { return ((int)DayOfWeek + 6) % 7; }
		}

		public int Y
		{
			get
			{
				var dateTime = new DateTime(Calendar.Year, (int)MonthType, No);
				return dateTime.GetWeekOfMonth();
			}
		}

		bool _isSelected;
		public bool IsSelected 
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				OnPropertyChanged(()=>IsSelected);
			}
		}
	}

	static class DateTimeExtensions
	{
		static GregorianCalendar _gc = new GregorianCalendar();
		public static int GetWeekOfMonth(this DateTime time)
		{
			var first = new DateTime(time.Year, time.Month, 1);
			return time.GetWeekOfYear() - first.GetWeekOfYear() + 1;
		}

		static int GetWeekOfYear(this DateTime time)
		{
			return _gc.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
		}
	}
}
