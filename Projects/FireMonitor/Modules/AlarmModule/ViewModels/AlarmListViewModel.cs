using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AlarmModule.Events;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace AlarmModule.ViewModels
{
    public class AlarmListViewModel : RegionViewModel
    {
        public AlarmListViewModel()
        {
            ResetAllCommand = new RelayCommand(OnResetAll, canReset);
            ServiceFactory.Events.GetEvent<ResetAlarmEvent>().Subscribe(OnResetAlarm);
            ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Subscribe(OnAlarmAdded);
            ServiceFactory.Events.GetEvent<MoveAlarmToEndEvent>().Subscribe(OnMoveAlarmToEnd);
        }

        AlarmType? _alarmType;

        public void Initialize(List<Alarm> alarms, AlarmType? alarmType)
        {
            _alarmType = alarmType;
            Alarms = new ObservableCollection<AlarmViewModel>();
            foreach (var alarm in alarms)
            {
                AlarmViewModel alarmViewModel = new AlarmViewModel();
                alarmViewModel.Initialize(alarm);
                Alarms.Add(alarmViewModel);
            }
        }

        ObservableCollection<AlarmViewModel> _alarms;
        public ObservableCollection<AlarmViewModel> Alarms
        {
            get { return _alarms; }
            set
            {
                _alarms = value;
                OnPropertyChanged("Alarms");
            }
        }

        AlarmViewModel _selectedAlarm;
        public AlarmViewModel SelectedAlarm
        {
            get { return _selectedAlarm; }
            set
            {
                _selectedAlarm = value;
                ServiceFactory.Layout.ShowAlarm(value);
                OnPropertyChanged("SelectedAlarm");
            }
        }

        bool canReset(object obj)
        {
            return Alarms.Count > 0;
        }

        public RelayCommand ResetAllCommand { get; private set; }
        void OnResetAll()
        {
            List<ResetItem> resetItems = new List<ResetItem>();

            foreach (var alarmViewModel in Alarms)
            {
                var resetItem = alarmViewModel.GetResetItem();
                if (resetItem != null)
                {
                    if (resetItems.Any(x => x.DeviceId == resetItem.DeviceId))
                    {
                        foreach (string stateName in resetItem.StateNames)
                        {
                            if (resetItem.StateNames.Contains(stateName) == false)
                            {
                                resetItem.StateNames.Add(stateName);
                            }
                        }
                    }
                    else
                    {
                        resetItems.Add(resetItem);
                    }
                }
            }

            FiresecManager.ResetStates(resetItems);
        }

        void OnAlarmAdded(Alarm alarm)
        {
            if ((_alarmType == null) || (alarm.AlarmType == _alarmType))
            {
                AlarmViewModel alarmViewModel = new AlarmViewModel();
                alarmViewModel.Initialize(alarm);
                Alarms.Insert(0, alarmViewModel);
            }
        }

        void OnResetAlarm(Alarm alarm)
        {
            if ((_alarmType == null) || (alarm.AlarmType == _alarmType))
            {
                var alarmViewModel = Alarms.FirstOrDefault(x => x._alarm.DeviceId == alarm.DeviceId);
                Alarms.Remove(alarmViewModel);
                if (Alarms.Count == 0)
                {
                    ServiceFactory.Layout.Close();
                }
            }
        }

        void OnMoveAlarmToEnd(AlarmViewModel alarmViewModel)
        {
            int oldIndex = Alarms.IndexOf(Alarms.FirstOrDefault(x => x.Name == alarmViewModel.Name));
            int newIndex = Alarms.Count;
            Alarms.Move(oldIndex, newIndex - 1);
        }
    }
}