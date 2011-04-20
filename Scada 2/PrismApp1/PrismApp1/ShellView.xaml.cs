﻿using System;
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

        bool isDevicesSelected;
        public bool IsDevicesSelected
        {
            get { return isDevicesSelected; }
            set
            {
                isDevicesSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowDevicesEvent>().Publish(null);
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
                    ServiceFactory.Events.GetEvent<ShowZonesEvent>().Publish(null);
                }
                OnPropertyChanged("IsZonesSelected");
            }
        }

        bool isReportSelectedSelected;
        public bool IsReportSelectedSelected
        {
            get { return isReportSelectedSelected; }
            set
            {
                isReportSelectedSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowReportsEvent>().Publish(null);
                }
                OnPropertyChanged("IsReportSelectedSelected");
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
            //string path = @"F8340ECE-C950-498D-88CD-DCBABBC604F3:Компьютер/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:6/1E045AD6-66F9-4F0B-901C-68C46C89E8DA:1.62";
            //ServiceFactory.Events.GetEvent<ShowDevicesEvent>().Publish(path);

            string path2 = @"F8340ECE-C950-498D-88CD-DCBABBC604F3:Компьютер/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:1/37F13667-BC77-4742-829B-1C43FA404C1F:1.17";
            ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Publish(path2);
        }
    }
}
