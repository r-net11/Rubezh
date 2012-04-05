using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AlarmModule.Events;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using System;

namespace AlarmModule.ViewModels
{
    public class AlarmsViewModel : RegionViewModel
    {
        public AlarmsViewModel(List<Alarm> alarms, AlarmType? alarmType)
        {
            ResetAllCommand = new RelayCommand(OnResetAll);
            RemoveAllFromIgnoreListCommand = new RelayCommand(OnRemoveAllFromIgnoreList, CanRemoveAllFromIgnoreList);
            ServiceFactory.Events.GetEvent<ResetAlarmEvent>().Subscribe(OnResetAlarm);
            ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Subscribe(OnAlarmAdded);
            ServiceFactory.Events.GetEvent<MoveAlarmToEndEvent>().Subscribe(OnMoveAlarmToEnd);

            _alarmType = alarmType;
            Alarms = new ObservableCollection<AlarmViewModel>(
                alarms.Select(alarm => new AlarmViewModel(alarm))
            );
        }

        AlarmType? _alarmType;

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

        public bool HasAlarmsToReset
        {
            get
            {
                return Alarms.Any(x => x.AlarmType != AlarmType.Off);
            }
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

        public bool CanRemoveAllFromIgnoreList()
        {
            return Alarms.Any(x => x.AlarmType == AlarmType.Off);
        }

        public RelayCommand RemoveAllFromIgnoreListCommand { get; private set; }
        void OnRemoveAllFromIgnoreList()
        {
            var deviceUIDs = new List<Guid>();
            foreach (var alarmViewModel in Alarms)
            {
                if (alarmViewModel.AlarmType == AlarmType.Off)
                {
                    var deviceUID = alarmViewModel.Alarm.DeviceUID;
                    var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == deviceUID);
                    if (deviceState.CanDisable() && deviceState.IsDisabled)

                        deviceUIDs.Add(deviceUID);
                }
            }

            FiresecManager.RemoveFromIgnoreList(deviceUIDs);
        }

        void OnAlarmAdded(Alarm alarm)
        {
            if (_alarmType == null || alarm.AlarmType == _alarmType)
            {
                Alarms.Insert(0, new AlarmViewModel(alarm));
                OnPropertyChanged("HasAlarmsToReset");
            }
        }

        void OnResetAlarm(Alarm alarm)
        {
            if (_alarmType == null || alarm.AlarmType == _alarmType)
            {
                Alarms.Remove(Alarms.FirstOrDefault(x => x.Alarm.DeviceUID == alarm.DeviceUID));
                OnPropertyChanged("HasAlarmsToReset");
            }
        }

        void OnMoveAlarmToEnd(AlarmViewModel alarmViewModel)
        {
            int oldIndex = Alarms.IndexOf(Alarms.FirstOrDefault(x => x.Description == alarmViewModel.Description));
            Alarms.Move(oldIndex, Alarms.Count - 1);
        }
    }
}