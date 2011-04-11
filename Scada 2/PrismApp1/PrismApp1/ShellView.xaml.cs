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

namespace PrismApp1
{
    /// <summary>
    /// Логика взаимодействия для Shell.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        public ShellView()
        {
            InitializeComponent();
            DataContext = this;
            ShowAlarmsCommand = new RelayCommand(OnShowAlarms);
            ShowPlansCommand = new RelayCommand(OnShowPlans);
            ShowJournalCommand = new RelayCommand(OnShowJournal);
            ShowDeviceParametersCommand = new RelayCommand(OnShowDeviceParameters);
            ShowDevicesCommand = new RelayCommand(OnShowDevices);
            ShowZonesCommand = new RelayCommand(OnShowZones);
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

        public RelayCommand ShowAlarmsCommand { get; private set; }
        void OnShowAlarms()
        {
            ServiceFactory.Events.GetEvent<ShowAllAlarmsEvent>().Publish(null);
        }

        public RelayCommand ShowPlansCommand { get; private set; }
        void OnShowPlans()
        {
            ServiceFactory.Events.GetEvent<ShowPlanEvent>().Publish(null);
        }

        public RelayCommand ShowJournalCommand { get; private set; }
        void OnShowJournal()
        {
            ServiceFactory.Events.GetEvent<ShowJournalEvent>().Publish(null);
        }

        public RelayCommand ShowDeviceParametersCommand { get; private set; }
        void OnShowDeviceParameters()
        {
        }

        public RelayCommand ShowDevicesCommand { get; private set; }
        void OnShowDevices()
        {
        }

        public RelayCommand ShowZonesCommand { get; private set; }
        void OnShowZones()
        {
        }
    }
}
