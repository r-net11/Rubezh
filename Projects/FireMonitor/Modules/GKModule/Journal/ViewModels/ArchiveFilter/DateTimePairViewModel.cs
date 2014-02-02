using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DateTimePairViewModel : BaseViewModel
	{
		public DateTimePairViewModel(DateTime dateTime)
		{
			Date = dateTime;
			Time = dateTime;
		}

		DateTime _date;
		public DateTime Date
		{
			get { return _date; }
			set
			{
				_date = value;
				OnPropertyChanged("Date");
			}
		}

		DateTime _time;
		public DateTime Time
		{
			get { return _time; }
			set
			{
				_time = value;
				OnPropertyChanged("Time");
			}
		}

		public DateTime DateTime
		{
			get { return new DateTime(Date.Year, Date.Month, Date.Day, Time.Hour, Time.Minute, Time.Second); }
		}
	}
}