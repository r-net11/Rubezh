using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Infrastructure;
using AlarmModule.Events;

namespace AlarmModule.ViewModels
{
    public class AlarmGroupDetailsViewModel : RegionViewModel
    {
        public AlarmGroupDetailsViewModel()
        {
            ServiceFactory.Events.GetEvent<ResetAlarmEvent>().Subscribe(OnResetAlarm);
            ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Subscribe(OnAlarmAdded);
        }

        void OnResetAlarm(Alarm alarm)
        {
            AlarmDetailsViewModel alarmDetailsViewModel = AlarmDetails.FirstOrDefault(x => x.alarm == alarm);
            AlarmDetails.Remove(alarmDetailsViewModel);
            if (AlarmDetails.Count == 0)
            {
                ServiceFactory.Layout.Close();
            }
        }

        void OnAlarmAdded(Alarm alarm)
        {
            AlarmDetailsViewModel alarmDetailsViewModel = new AlarmDetailsViewModel();
            alarmDetailsViewModel.Initialize(alarm);
            AlarmDetails.Add(alarmDetailsViewModel);
        }

        public void Initialize(List<Alarm> alarms)
        {
            AlarmDetails = new ObservableCollection<AlarmDetailsViewModel>();
            foreach (Alarm alarm in alarms)
            {
                AlarmDetailsViewModel alarmDetailsViewModel = new AlarmDetailsViewModel();
                alarmDetailsViewModel.Initialize(alarm);
                AlarmDetails.Add(alarmDetailsViewModel);
            }
        }

        ObservableCollection<AlarmDetailsViewModel> alarmDetails;
        public ObservableCollection<AlarmDetailsViewModel> AlarmDetails
        {
            get { return alarmDetails; }
            set
            {
                alarmDetails = value;
                OnPropertyChanged("AlarmDetails");
            }
        }

        public override void Dispose()
        {
            ServiceFactory.Events.GetEvent<ResetAlarmEvent>().Unsubscribe(OnResetAlarm);
            ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Unsubscribe(OnAlarmAdded);
        }
    }
}
