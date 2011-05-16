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
    /// <summary>
    /// Логика взаимодействия для NavigationView.xaml
    /// </summary>
    public partial class NavigationView : UserControl, INotifyPropertyChanged
    {
        public NavigationView()
        {
            InitializeComponent();

            DataContext = this;

            ServiceFactory.Events.GetEvent<ShowPlanEvent>().Subscribe(x => { isPlanSelected = true; OnPropertyChanged("IsPlanSelected"); });
            ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Subscribe(x => { isPlanSelected = true; OnPropertyChanged("IsPlanSelected"); });
            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Subscribe(x => { isDevicesSelected = true; OnPropertyChanged("IsDevicesSelected"); });
            ServiceFactory.Events.GetEvent<ShowZoneEvent>().Subscribe(x => { isZonesSelected = true; OnPropertyChanged("IsZonesSelected"); });
            ServiceFactory.Events.GetEvent<ShowJournalEvent>().Subscribe(x => { isJournalSelected = true; OnPropertyChanged("IsJournalSelected"); });
            ServiceFactory.Events.GetEvent<ShowReportsEvent>().Subscribe(x => { isReportSelected = true; OnPropertyChanged("IsReportSelected"); });
            ServiceFactory.Events.GetEvent<ShowCallEvent>().Subscribe(x => { isCallSelected = true; OnPropertyChanged("IsCallSelected"); });
            ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Subscribe(x => { isArchiveSelected = true; OnPropertyChanged("IsArchiveSelected"); });
        }

        bool isAlarmSelected;
        public bool IsAlarmSelected
        {
            get { return isAlarmSelected; }
            set
            {
                isAlarmSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowAllAlarmsEvent>().Publish(null);
                }
                OnPropertyChanged("IsAlarmSelected");
            }
        }

        bool isPlanSelected;
        public bool IsPlanSelected
        {
            get { return isPlanSelected; }
            set
            {
                isPlanSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowPlanEvent>().Publish(null);
                }
                OnPropertyChanged("IsPlanSelected");
            }
        }

        bool isJournalSelected;
        public bool IsJournalSelected
        {
            get { return isJournalSelected; }
            set
            {
                isJournalSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowJournalEvent>().Publish(null);
                }
                OnPropertyChanged("IsJournalSelected");
            }
        }

        bool isDevicesSelected;
        public bool IsDevicesSelected
        {
            get { return isDevicesSelected; }
            set
            {
                isDevicesSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(null);
                }
                OnPropertyChanged("IsDevicesSelected");
            }
        }

        bool isZonesSelected;
        public bool IsZonesSelected
        {
            get { return isZonesSelected; }
            set
            {
                isZonesSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(null);
                }
                OnPropertyChanged("IsZonesSelected");
            }
        }

        bool isReportSelected;
        public bool IsReportSelected
        {
            get { return isReportSelected; }
            set
            {
                isReportSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowReportsEvent>().Publish(null);
                }
                OnPropertyChanged("IsReportSelected");
            }
        }

        bool isArchiveSelected;
        public bool IsArchiveSelected
        {
            get { return isArchiveSelected; }
            set
            {
                isArchiveSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(null);
                }
                OnPropertyChanged("IsArchiveSelected");
            }
        }

        bool isCallSelected;
        public bool IsCallSelected
        {
            get { return isCallSelected; }
            set
            {
                isCallSelected = value;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string id = @"F8340ECE-C950-498D-88CD-DCBABBC604F3:Компьютер/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:1/37F13667-BC77-4742-829B-1C43FA404C1F:1.17";
            ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Publish(id);
        }
    }
}
