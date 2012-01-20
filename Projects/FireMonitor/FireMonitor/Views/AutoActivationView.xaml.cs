using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;

namespace FireMonitor.Views
{
    public partial class AutoActivationView : UserControl, INotifyPropertyChanged
    {
        public AutoActivationView()
        {
            InitializeComponent();
            DataContext = this;

            ChangeAutoActivationCommand = new RelayCommand(OnChangeAutoActivation);
            ChangePlansAutoActivationCommand = new RelayCommand(OnChangePlansAutoActivation);
            FiresecEventSubscriber.NewJournalRecordEvent += new Action<JournalRecord>(OnNewJournalRecord);
        }

        public bool IsAutoActivation
        {
            get { return ClientSettings.AutoActivationSettings.IsAutoActivation; }
            set
            {
                ClientSettings.AutoActivationSettings.IsAutoActivation = value;
                OnPropertyChanged("IsAutoActivation");
            }
        }

        public bool IsPlansAutoActivation
        {
            get { return ClientSettings.AutoActivationSettings.IsPlansAutoActivation; }
            set
            {
                ClientSettings.AutoActivationSettings.IsPlansAutoActivation = value;
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
            OnPropertyChanged("IsPlansAutoActivation");
        }

        void OnNewJournalRecord(JournalRecord journalRecord)
        {
            if (IsAutoActivation)
            {
                if ((App.Current.MainWindow != null) && (App.Current.MainWindow.IsActive == false))
                {
                    App.Current.MainWindow.Activate();
                }
            }
            if (IsPlansAutoActivation)
            {
                if (string.IsNullOrWhiteSpace(journalRecord.DeviceDatabaseId) == false)
                {
                    var stateType = StateType.No;
                    foreach (var deviceState in FiresecManager.DeviceStates.DeviceStates)
                    {
                        if (deviceState.StateType < stateType)
                            stateType = deviceState.StateType;
                    }

                    var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.DatabaseId == journalRecord.DeviceDatabaseId);
                    if (device != null)
                    {
                        var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == device.UID);
                        if (deviceState.StateType <= stateType)
                            ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Publish(device.UID);
                    }
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