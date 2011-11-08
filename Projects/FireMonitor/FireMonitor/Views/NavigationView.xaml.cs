using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using AlarmModule.Events;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Events;

namespace FireMonitor
{
    public partial class NavigationView : UserControl, INotifyPropertyChanged
    {
        public NavigationView()
        {
            InitializeComponent();

            DataContext = this;

            ServiceFactory.Events.GetEvent<ShowNothingEvent>().Subscribe(x => { DeselectAll(); });
            ServiceFactory.Events.GetEvent<ShowAlarmsEvent>().Subscribe(x => { _isAlarmSelected = true; OnPropertyChanged("IsAlarmSelected"); });
            ServiceFactory.Events.GetEvent<ShowPlansEvent>().Subscribe(x => { _isPlanSelected = true; OnPropertyChanged("IsPlanSelected"); });
            ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Subscribe(x => { _isPlanSelected = true; OnPropertyChanged("IsPlanSelected"); });
            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Subscribe(x => { _isDevicesSelected = true; OnPropertyChanged("IsDevicesSelected"); });
            ServiceFactory.Events.GetEvent<ShowZoneEvent>().Subscribe(x => { _isZonesSelected = true; OnPropertyChanged("IsZonesSelected"); });
            ServiceFactory.Events.GetEvent<ShowJournalEvent>().Subscribe(x => { _isJournalSelected = true; OnPropertyChanged("IsJournalSelected"); });
            ServiceFactory.Events.GetEvent<ShowReportsEvent>().Subscribe(x => { _isReportSelected = true; OnPropertyChanged("IsReportSelected"); });
            ServiceFactory.Events.GetEvent<ShowCallEvent>().Subscribe(x => { _isCallSelected = true; OnPropertyChanged("IsCallSelected"); });
            ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Subscribe(x => { _isArchiveSelected = true; OnPropertyChanged("IsArchiveSelected"); });
        }

        void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FiresecEventSubscriber.NewJournalRecordEvent += new Action<JournalRecord>(OnNewJournalItemEvent);
            //IsAlarmSelected = true;
        }

        void DeselectAll()
        {
            IsAlarmSelected = IsPlanSelected = IsDevicesSelected = IsZonesSelected = IsJournalSelected = IsReportSelected = IsCallSelected = IsArchiveSelected = false;
        }

        int _unreadJournalCount = 0;
        public int UnreadJournalCount
        {
            get { return _unreadJournalCount; }
            set
            {
                _unreadJournalCount = value;
                HasUnreadJournal = (value > 0);
                OnPropertyChanged("UnreadJournalCount");
            }
        }

        bool _hasUnreadJournal = false;
        public bool HasUnreadJournal
        {
            get { return _hasUnreadJournal; }
            set
            {
                _hasUnreadJournal = value;
                OnPropertyChanged("HasUnreadJournal");
            }
        }

        void OnNewJournalItemEvent(JournalRecord journalItem)
        {
            if (IsJournalSelected == false)
            {
                ++UnreadJournalCount;
            }
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
                    ServiceFactory.Events.GetEvent<ShowAlarmsEvent>().Publish(null);
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
                    ServiceFactory.Events.GetEvent<ShowPlansEvent>().Publish(null);
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
                    UnreadJournalCount = 0;
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
                    ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Guid.Empty);
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