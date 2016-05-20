using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutomationModule.ViewModels
{
	public class DateTimeViewModel : BaseViewModel
	{
		public DateTimeViewModel(DateTime dateTime)
		{
			Value = dateTime;
		}

		DateTime _value;
		public DateTime Value
		{
			get { return _value; }
			set 
			{
				_value = value;
				OnPropertyChanged(() => Value);
			}
		}

		public TimeSpan TimeOfDay
		{
			get { return Value.TimeOfDay; }
			set 
			{
				Value = new DateTime(Value.Year, Value.Month, Value.Day, value.Hours, value.Minutes, 0);
				OnPropertyChanged(() => TimeOfDay);
			}
		}
	}
}