using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using Infrastructure;
using Infrastructure.Events;
using FiresecClient;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class AlarmsViewModel : ViewPartViewModel
	{
		public AlarmsViewModel()
		{
			Alarms = new ObservableCollection<AlarmViewModel>();
			ServiceFactory.Events.GetEvent<GKObjectsStateChangedEvent>().Unsubscribe(OnGKObjectsStateChanged);
			ServiceFactory.Events.GetEvent<GKObjectsStateChangedEvent>().Subscribe(OnGKObjectsStateChanged);
		}

		public ObservableCollection<AlarmViewModel> Alarms { get; private set; }

		AlarmViewModel _selectedAlarm;
		public AlarmViewModel SelectedAlarm
		{
			get { return _selectedAlarm; }
			set
			{
				_selectedAlarm = value;
				OnPropertyChanged("SelectedAlarm");
			}
		}

		void OnGKObjectsStateChanged(object obj)
		{
			var alarms = new List<Alarm>();
			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				foreach (var stateType in device.DeviceState.States)
				{
					switch (stateType)
					{
						case XStateType.Fire1:
							alarms.Add(new Alarm()
							{
								AlarmType = XAlarmType.Fire1,
								StateType = stateType,
								Device = device
							});
							break;

						case XStateType.Fire2:
							alarms.Add(new Alarm()
							{
								AlarmType = XAlarmType.Fire2,
								StateType = stateType,
								Device = device
							});
							break;

						case XStateType.Attention:
							alarms.Add(new Alarm()
							{
								AlarmType = XAlarmType.Attention,
								StateType = stateType,
								Device = device
							});
							break;

						case XStateType.Ignore:
							alarms.Add(new Alarm()
							{
								AlarmType = XAlarmType.Ignore,
								StateType = stateType,
								Device = device
							});
							break;
					}
				}
			}

			Alarms.Clear();
			foreach (var alarm in alarms)
			{
				var alarmViewModel = new AlarmViewModel(alarm);
				Alarms.Add(alarmViewModel);
			}

			AlarmsGroupsViewModel.Current.Update(alarms);
		}

		public void Sort(XStateType? stateType)
		{

		}
	}
}