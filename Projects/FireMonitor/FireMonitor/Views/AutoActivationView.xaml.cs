using System;
using System.Linq;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;

namespace FireMonitor
{
    public partial class AutoActivationView : UserControl, INotifyPropertyChanged
    {
        public AutoActivationView()
        {
            InitializeComponent();
            DataContext = this;

            ChangeAutoActivationCommand = new RelayCommand(OnChangeAutoActivation);
            ChangePlansAutoActivationCommand = new RelayCommand(OnChangePlansAutoActivation);
            IsAutoActivation = true;
            IsPlansAutoActivation = true;
            FiresecEventSubscriber.NewJournalRecordEvent += new Action<JournalRecord>(OnNewJournalRecord);
        }

        bool _isAutoActivation;
        public bool IsAutoActivation
        {
            get { return _isAutoActivation; }
            set
            {
                _isAutoActivation = value;
                OnPropertyChanged("IsAutoActivation");
            }
        }

        bool _isPlansAutoActivation;
        public bool IsPlansAutoActivation
        {
            get { return _isPlansAutoActivation; }
            set
            {
                _isPlansAutoActivation = value;
                OnPropertyChanged("IsPlansAutoActivation");
            }
        }

        public RelayCommand ChangeAutoActivationCommand { get; private set; }
        void OnChangeAutoActivation()
        {
            IsAutoActivation = !IsAutoActivation;
        }

        public RelayCommand ChangePlansAutoActivationCommand { get; private set; }
        void OnChangePlansAutoActivation()
        {
            IsPlansAutoActivation = !IsPlansAutoActivation;
        }

        void OnNewJournalRecord(JournalRecord journalRecord)
        {
            if (IsAutoActivation)
            {
                if (!App.Current.MainWindow.IsActive)
                {
                    //App.Current.MainWindow.WindowState = WindowState.Maximized;
                    App.Current.MainWindow.Activate();
                }
            }
            if (IsPlansAutoActivation)
            {
                if (string.IsNullOrWhiteSpace(journalRecord.DeviceDatabaseId) == false)
                {
                    var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.DatabaseId == journalRecord.DeviceDatabaseId);
                    if (device != null)
                        ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Publish(device.UID);
                }
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