using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
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

        public void Initialize(List<Alarm> alarms)
        {
            AlarmDetails = new ObservableCollection<AlarmDetailsViewModel>();
            foreach (Alarm alarm in alarms)
            {
                AlarmDetailsViewModel alarmDetailsViewModel = new AlarmDetailsViewModel();
                alarmDetailsViewModel.Initialize(alarm, true);
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
        }
    }
}
