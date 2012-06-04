using System.Collections.Generic;
using System.Linq;
using AlarmModule.Events;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Infrastructure.Common.Windows.ViewModels;

namespace AlarmModule.ViewModels
{
	public class AlarmGroupViewModel : ViewPartViewModel
	{
		public AlarmGroupViewModel()
		{
			Alarms = new List<Alarm>();
			ShowCommand = new RelayCommand(OnShowCommand);
			ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Subscribe(OnAlarmAdded);
			ServiceFactory.Events.GetEvent<AlarmRemovedEvent>().Subscribe(OnResetAlarm);
		}

		public AlarmType AlarmType { get; set; }
		public List<Alarm> Alarms { get; set; }

		void OnAlarmAdded(Alarm alarm)
		{
			if (alarm.AlarmType == AlarmType)
				Alarms.Add(alarm);
			Update();
		}

		void OnResetAlarm(Alarm alarm)
		{
			if (alarm.AlarmType == this.AlarmType)
				Alarms.Remove(Alarms.FirstOrDefault(x => x.DeviceUID == alarm.DeviceUID));
			Update();
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShowCommand()
		{
			ServiceFactory.Events.GetEvent<ShowAlarmsEvent>().Publish(AlarmType);
			ServiceFactory.Events.GetEvent<ShowNothingEvent>().Publish(null);
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