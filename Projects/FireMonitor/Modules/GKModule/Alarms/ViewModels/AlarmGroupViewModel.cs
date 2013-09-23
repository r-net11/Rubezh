using System.Collections.Generic;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class AlarmGroupViewModel : BaseViewModel
	{
		public AlarmGroupViewModel(XAlarmType alarmType)
		{
			Alarms = new List<AlarmViewModel>();
			ShowCommand = new RelayCommand(OnShowCommand);
			AlarmType = alarmType;
		}

		public XAlarmType AlarmType { get; set; }
		public List<AlarmViewModel> Alarms { get; set; }

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