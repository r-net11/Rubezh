using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Infrastructure;
using Infrastructure.Events;
using AlarmModule.Events;
using System.ComponentModel;

namespace FireMonitor
{
    public partial class NavigationView : UserControl, INotifyPropertyChanged
    {
        public NavigationView()
        {
            InitializeComponent();

            DataContext = this;

            ServiceFactory.Events.GetEvent<ShowNothingEvent>().Subscribe(x => { DeselectAll(); });
            ServiceFactory.Events.GetEvent<ShowPlanEvent>().Subscribe(x => { _isPlanSelected = true; OnPropertyChanged("IsPlanSelected"); });
            ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Subscribe(x => { _isPlanSelected = true; OnPropertyChanged("IsPlanSelected"); });
            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Subscribe(x => { _isDevicesSelected = true; OnPropertyChanged("IsDevicesSelected"); });
            ServiceFactory.Events.GetEvent<ShowZoneEvent>().Subscribe(x => { _isZonesSelected = true; OnPropertyChanged("IsZonesSelected"); });
            ServiceFactory.Events.GetEvent<ShowJournalEvent>().Subscribe(x => { _isJournalSelected = true; OnPropertyChanged("IsJournalSelected"); });
            ServiceFactory.Events.GetEvent<ShowReportsEvent>().Subscribe(x => { _isReportSelected = true; OnPropertyChanged("IsReportSelected"); });
            ServiceFactory.Events.GetEvent<ShowCallEvent>().Subscribe(x => { _isCallSelected = true; OnPropertyChanged("IsCallSelected"); });
            ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Subscribe(x => { _isArchiveSelected = true; OnPropertyChanged("IsArchiveSelected"); });
        }

        void DeselectAll()
        {
            IsAlarmSelected = IsPlanSelected = IsDevicesSelected = IsZonesSelected = IsJournalSelected = IsReportSelected = IsCallSelected = IsArchiveSelected = false;
        }

        bool _isAlarmSelected;
        public bool IsAlarmSelected
        {
            get { return _isAlarmSelected; }
            set
            {
                _isAlarmSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowAllAlarmsEvent>().Publish(null);
                }
                OnPropertyChanged("IsAlarmSelected");
            }
        }

        bool _isPlanSelected;
        public bool IsPlanSelected
        {
            get { return _isPlanSelected; }
            set
            {
                _isPlanSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowPlanEvent>().Publish(null);
                }
                OnPropertyChanged("IsPlanSelected");
            }
        }

        bool _isJournalSelected;
        public bool IsJournalSelected
        {
            get { return _isJournalSelected; }
            set
            {
                _isJournalSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowJournalEvent>().Publish(null);
                }
                OnPropertyChanged("IsJournalSelected");
            }
        }

        bool _isDevicesSelected;
        public bool IsDevicesSelected
        {
            get { return _isDevicesSelected; }
            set
            {
                _isDevicesSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(null);
                }
                OnPropertyChanged("IsDevicesSelected");
            }
        }

        bool _isZonesSelected;
        public bool IsZonesSelected
        {
            get { return _isZonesSelected; }
            set
            {
                _isZonesSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(null);
                }
                OnPropertyChanged("IsZonesSelected");
            }
        }

        bool _isReportSelected;
        public bool IsReportSelected
        {
            get { return _isReportSelected; }
            set
            {
                _isReportSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowReportsEvent>().Publish(null);
                }
                OnPropertyChanged("IsReportSelected");
            }
        }

        bool _isArchiveSelected;
        public bool IsArchiveSelected
        {
            get { return _isArchiveSelected; }
            set
            {
                _isArchiveSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(null);
                }
                OnPropertyChanged("IsArchiveSelected");
            }
        }

        bool _isCallSelected;
        public bool IsCallSelected
        {
            get { return _isCallSelected; }
            set
            {
                _isCallSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowCallEvent>().Publish(null);
                }
                OnPropertyChanged("IsCallSelected");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
