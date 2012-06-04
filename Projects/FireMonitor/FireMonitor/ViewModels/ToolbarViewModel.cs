using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace FireMonitor.ViewModels
{
	public class ToolbarViewModel : BaseViewModel
	{
		private BaseViewModel _alarmGroups;
		public BaseViewModel AlarmGroups
		{
			get { return _alarmGroups; }
			set
			{
				_alarmGroups = value;
				OnPropertyChanged("AlarmGroups");
			}
		}
	}
}
