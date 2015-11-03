using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class DateTimePairViewModel : BaseViewModel
	{
		public DateTimePairViewModel(DateTime dateTime)
		{
			Date = dateTime.Date;
			Time = dateTime.TimeOfDay;
		}

		DateTime _date;
		public DateTime Date
		{
			get { return _date; }
			set
			{
				_date = value;
				OnPropertyChanged(() => Date);
			}
		}

		TimeSpan _time;
		public TimeSpan Time
		{
			get { return _time; }
			set
			{
				_time = value;
				OnPropertyChanged(() => Time);
			}
		}

		public DateTime DateTime
		{
			get { return new DateTime(Date.Year, Date.Month, Date.Day, Time.Hours, Time.Minutes, Time.Seconds); }
		}
	}
}
