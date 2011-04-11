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
using System.Windows.Shapes;
using Infrastructure;
using AlarmModule.Events;
using Infrastructure.Events;
using System.Diagnostics;
using System.ComponentModel;

namespace PrismApp1
{
    /// <summary>
    /// Логика взаимодействия для Shell.xaml
    /// </summary>
    public partial class ShellView : Window, INotifyPropertyChanged
    {
        public ShellView()
        {
            InitializeComponent();
            DataContext = this;
        }

        public IViewPart MainContent
        {
            get { return _mainRegionHost.Content as IViewPart; }
            set { _mainRegionHost.DataContext = _mainRegionHost.Content = value; }
        }

        public IViewPart AlarmGroups
        {
            get { return _alarmGroups.Content as IViewPart; }
            set { _alarmGroups.DataContext = _alarmGroups.Content = value; }
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

        bool isDeviceParametersSelectedSelected;
        public bool IsDeviceParametersSelectedSelected
        {
            get { return isDeviceParametersSelectedSelected; }
            set
            {
                isDeviceParametersSelectedSelected = value;
                if (value)
                {
                    //ServiceFactory.Events.GetEvent<ShowJournalEvent>().Publish(null);
                }
                OnPropertyChanged("IsDeviceParametersSelectedSelected");
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
                    //ServiceFactory.Events.GetEvent<ShowJournalEvent>().Publish(null);
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
                    //ServiceFactory.Events.GetEvent<ShowJournalEvent>().Publish(null);
                }
                OnPropertyChanged("IsZonesSelected");
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
