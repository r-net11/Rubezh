using System.Collections.Generic;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace AlarmModule.ViewModels
{
	public class AlarmGroupViewModel : ViewPartViewModel
	{
		public AlarmGroupViewModel()
		{
			Alarms = new List<Alarm>();
			ShowCommand = new RelayCommand(OnShowCommand);
		}

		public AlarmType AlarmType { get; set; }
		public List<Alarm> Alarms { get; set; }

		public RelayCommand ShowCommand { get; private set; }
		void OnShowCommand()
		{
			ServiceFactory.Events.GetEvent<ShowAlarmsEvent>().Publish(AlarmType);
			ServiceFactory.Events.GetEvent<ShowNothingEvent>().Publish(null);
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