using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure;
using Infrastructure.Events;
using Infrastructure.Common;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class AlarmGroupViewModel : BaseViewModel
	{
		public AlarmGroupViewModel(XAlarmType alarmType)
		{
			Alarms = new List<Alarm>();
			ShowCommand = new RelayCommand(OnShowCommand);
			AlarmType = alarmType;
			StateType = AlarmToStateHelper.AlarmToState(alarmType);
		}

		public XStateType StateType { get; set; }
		public XAlarmType AlarmType { get; set; }
		public List<Alarm> Alarms { get; set; }

		public RelayCommand ShowCommand { get; private set; }
		void OnShowCommand()
		{
			ServiceFactory.Events.GetEvent<ShowXAlarmsEvent>().Publish(AlarmType);
		}

		public void Update()
		{
			OnPropertyChanged("Alarms");
			OnPropertyChanged("Count");
			OnPropertyChanged("HasAlarms");
		}

		public int Count
		{
			get { return Alarms.Count; }
		}

		public bool HasAlarms
		{
			get { return (Count > 0); }
		}
	}
}