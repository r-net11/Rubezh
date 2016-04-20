using System.Collections.Generic;
using RubezhAPI.GK;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class AlarmGroupViewModel : BaseViewModel
	{
		public AlarmGroupViewModel(GKAlarmType alarmType)
		{
			Alarms = new List<AlarmViewModel>();
			ShowCommand = new RelayCommand(OnShowCommand);
			AlarmType = alarmType;
		}

		public GKAlarmType AlarmType { get; set; }
		public List<AlarmViewModel> Alarms { get; set; }

		public RelayCommand ShowCommand { get; private set; }
		void OnShowCommand()
		{
			ServiceFactory.Events.GetEvent<ShowGKAlarmsEvent>().Publish(AlarmType);
		}

		public void Update()
		{
			OnPropertyChanged(() => Alarms);
			OnPropertyChanged(() => Count);
			OnPropertyChanged(() => HasAlarms);
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