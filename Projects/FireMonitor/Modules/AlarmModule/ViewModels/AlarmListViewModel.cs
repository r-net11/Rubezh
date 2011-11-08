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
        public AlarmListViewModel(List<Alarm> alarms, AlarmType? alarmType)
        {
            _alarmType = alarmType;
            Alarms = new ObservableCollection<AlarmViewModel>(
                alarms.Select(alarm => new AlarmViewModel(alarm))
            );

            ResetAllCommand = new RelayCommand(OnResetAll, canReset);
            ServiceFactory.Events.GetEvent<ResetAlarmEvent>().Subscribe(OnResetAlarm);
            ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Subscribe(OnAlarmAdded);
            ServiceFactory.Events.GetEvent<MoveAlarmToEndEvent>().Subscribe(OnMoveAlarmToEnd);
        }

        AlarmType? _alarmType;

        public ObservableCollection<AlarmViewModel> Alarms { get; set; }

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

        bool canReset()
        {
            return Alarms.Count > 0;
        }

        public RelayCommand ResetAllCommand { get; private set; }
        void OnResetAll()
        {
            var resetItems = new List<ResetItem>();
            foreach (var alarmViewModel in Alarms)
            {
                var resetItem = alarmViewModel.Alarm.GetResetItem();
                if (resetItem != null)
                {
                    var existringResetItem = resetItems.FirstOrDefault(x => x.DeviceUID == resetItem.DeviceUID);
                    if (existringResetItem != null)
                    {
                        foreach (string stateName in resetItem.StateNames)
                        {
                            if (existringResetItem.StateNames.Contains(stateName) == false)
                                existringResetItem.StateNames.Add(stateName);
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
            if (_alarmType == null || alarm.AlarmType == _alarmType)
                Alarms.Insert(0, new AlarmViewModel(alarm));
        }

        void OnResetAlarm(Alarm alarm)
        {
            if (_alarmType == null || alarm.AlarmType == _alarmType)
            {
                Alarms.Remove(Alarms.FirstOrDefault(x => x.Alarm.DeviceUID == alarm.DeviceUID));
                if (Alarms.Count == 0)
                    ServiceFactory.Layout.Close();
            }
        }

        void OnMoveAlarmToEnd(AlarmViewModel alarmViewModel)
        {
            int oldIndex = Alarms.IndexOf(Alarms.FirstOrDefault(x => x.Description == alarmViewModel.Description));
            Alarms.Move(oldIndex, Alarms.Count - 1);
        }
    }
}