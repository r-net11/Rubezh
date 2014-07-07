using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.SKD;
using System.Collections.ObjectModel;

namespace SKDModule.ViewModels
{
	public class LockIntervalsViewModel : BaseViewModel
	{
		public LockIntervalsViewModel(SKDDoorConfiguration doorConfiguration)
		{
			DayIntervals = new ObservableCollection<DayInterval>();
			for (int i = 1; i <= 7; i++)
			{
				var dayInterval = new DayInterval(i);
				DayIntervals.Add(dayInterval);
			}
		}

		public ObservableCollection<DayInterval> DayIntervals { get; private set; }

		public class DayInterval : BaseViewModel
		{
			public DayInterval(int dayNo)
			{
				Name = IntToWeekDay(dayNo);
				Intervals = new ObservableCollection<Interval>();
				for (int i = 0; i < 4; i++)
				{
					var interval = new Interval();
					Intervals.Add(interval);
				}
			}

			public string Name { get; private set; }
			public ObservableCollection<Interval> Intervals { get; private set; }

			string IntToWeekDay(int dayNo)
			{
				switch (dayNo)
				{
					case 1:
						return "Понедельник";
					case 2:
						return "Вторник";
					case 3:
						return "Среда";
					case 4:
						return "Четверг";
					case 5:
						return "Пятница";
					case 6:
						return "Суббота";
					case 7:
						return "Воскресенье";
				}
				return "Неизвестный день";
			}
		}

		public class Interval : BaseViewModel
		{
			public Interval()
			{
				StartHours = new ObservableCollection<int>();
				StartMinutes = new ObservableCollection<int>();
				EndHours = new ObservableCollection<int>();
				EndMinutes = new ObservableCollection<int>();

				for (int i = 0; i <= 23; i++)
				{
					StartHours.Add(i);
					EndHours.Add(i);
				}
				for (int i = 0; i <= 59; i++)
				{
					StartMinutes.Add(i);
					EndMinutes.Add(i);
				}
			}

			public ObservableCollection<int> StartHours { get; private set; }
			public ObservableCollection<int> StartMinutes { get; private set; }
			public ObservableCollection<int> EndHours { get; private set; }
			public ObservableCollection<int> EndMinutes { get; private set; }

			public int StartHour { get; set; }
			public int StartMinute { get; set; }
			public int EndHour { get; set; }
			public int EndMinute { get; set; }
		}
	}
}