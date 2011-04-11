using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Collections.ObjectModel;

namespace WpfApplication4
{
    public class MainAlarm : BaseViewModel
    {
        public MainAlarm()
        {
            ShowCommand = new RelayCommand(OnShow);
        }

        public RelayCommand ShowCommand { get; private set; }
        void OnShow(object obj)
        {
            AlarmsView alarmsView = new AlarmsView();
            alarmsView.DataContext = this;
            ViewModel.Current.Content = alarmsView;
        }

        public AlarmType AlarmType { get; set; }

        string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public void Update()
        {
            OnPropertyChanged("Alarms");
            OnPropertyChanged("Count");
            OnPropertyChanged("HasAlarms");
        }

        public ObservableCollection<DetailAlarm> Alarms
        {
            get
            {
                ObservableCollection<DetailAlarm> alarms = new ObservableCollection<DetailAlarm>();
                foreach (DetailAlarm detailAlarm in ViewModel.Current.DetailAlarms)
                {
                    if (detailAlarm.AlarmType == AlarmType)
                        alarms.Add(detailAlarm);
                }
                return alarms;
            }
        }

        public int Count
        {
            get
            {
                return Alarms.Count;
            }
        }

        public bool HasAlarms
        {
            get
            {
                return (Count > 0);
            }
        }
    }
}
