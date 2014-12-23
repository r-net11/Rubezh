using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;

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
		}

		public int Year
		{
			get { return Calendar.Year; }
			set
			{
				Calendar.Year = value;
				UpdateMonth();
				OnPropertyChanged(() => Year);
			}
		}

		void UpdateMonth()
		{
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
			Days = new List<DayViewModel>();
			for (int i = 1; i <= 31; i++)
			{
				if ((monthType == MonthType.April || monthType == MonthType.June || monthType == MonthType.September || monthType == MonthType.November)&&(i == 31))
					break;
				if (monthType == MonthType.February && Calendar.Year % 4 == 0 && i == 30)
					break;
				if (monthType == MonthType.February && Calendar.Year % 4 != 0 && i == 29)
					break;
				Days.Add(new DayViewModel(i, MonthType, Calendar));
			}
		}

		public ObservableCollection<DayViewModel> Mondays { get { return new ObservableCollection<DayViewModel>(Days.FindAll(x => x.DayOfWeek == DayOfWeek.Monday)); }}
		public ObservableCollection<DayViewModel> Tuesdays { get { return new ObservableCollection<DayViewModel>(Days.FindAll(x => x.DayOfWeek == DayOfWeek.Tuesday)); } }
		public ObservableCollection<DayViewModel> Wednesdays { get { return new ObservableCollection<DayViewModel>(Days.FindAll(x => x.DayOfWeek == DayOfWeek.Wednesday)); } }
		public ObservableCollection<DayViewModel> Thursdays { get { return new ObservableCollection<DayViewModel>(Days.FindAll(x => x.DayOfWeek == DayOfWeek.Thursday)); } }
		public ObservableCollection<DayViewModel> Fridays { get { return new ObservableCollection<DayViewModel>(Days.FindAll(x => x.DayOfWeek == DayOfWeek.Friday)); } }
		public ObservableCollection<DayViewModel> Saturdays { get { return new ObservableCollection<DayViewModel>(Days.FindAll(x => x.DayOfWeek == DayOfWeek.Saturday)); } }
		public ObservableCollection<DayViewModel> Sundays { get { return new ObservableCollection<DayViewModel>(Days.FindAll(x => x.DayOfWeek == DayOfWeek.Sunday)); } }
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
		}

		bool _isSelected;
		public bool IsSelected 
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				var dateTime = new DateTime(Calendar.Year, (int)MonthType, No);
				if (_isSelected)
					Calendar.SelectedDays.Add(dateTime);
				else
					Calendar.SelectedDays.Remove(dateTime);
				OnPropertyChanged(()=>IsSelected);
			}
		}
	}
}
