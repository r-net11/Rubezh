using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Infrastructure;
using Infrastructure.Common;
using AlarmModule.Events;
using FiresecClient;
using FiresecAPI.Models;

namespace AlarmModule.ViewModels
{
    public class AlarmListViewModel : RegionViewModel
    {
        public AlarmListViewModel()
        {
            ResetAllCommand = new RelayCommand(OnResetAll);
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
                        foreach (string state in resetItem.States)
                        {
                            if (resetItem.States.Contains(state) == false)
                            {
                                resetItem.States.Add(state);
                            }
                        }
                    }
                    else
                    {
                        resetItems.Add(resetItem);
                    }
                }
            }

            FiresecManager.ResetMany(resetItems);
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
                AlarmViewModel alarmViewModel = Alarms.FirstOrDefault(x => x._alarm.DeviceId == alarm.DeviceId);
                Alarms.Remove(alarmViewModel);
                if (Alarms.Count == 0)
                {
                    ServiceFactory.Layout.Close();
                }
            }
        }

        void OnMoveAlarmToEnd(AlarmViewModel alarmViewModel)
        {
            //int oldIndex = Alarms.IndexOf(alarmViewModel);
            int oldIndex = Alarms.IndexOf(Alarms.FirstOrDefault(x=>x.Name == alarmViewModel.Name));
            int newIndex = Alarms.Count;
            Alarms.Move(oldIndex, newIndex - 1);
        }
    }
}
