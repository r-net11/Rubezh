using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FiresecAPI.Models;
using FiresecClient;

namespace FireMonitor
{
    public partial class ConnectionIndicatorView : UserControl, INotifyPropertyChanged
    {
        public ConnectionIndicatorView()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            IsServiceConnected = true;
            SafeFiresecService.ConnectionLost += new Action(OnConnectionLost);
            SafeFiresecService.ConnectionAppeared += new Action(OnConnectionAppeared);

            OnDeviceStateChangedEvent(Guid.Empty);
            FiresecEventSubscriber.DeviceStateChangedEvent += new Action<Guid>(OnDeviceStateChangedEvent);
        }

        void OnDeviceStateChangedEvent(Guid deviceUID)
        {
            IsDeviceConnected = FiresecManager.DeviceStates.DeviceStates.Any(x => x.StateType == StateType.Unknown) == false;
        }

        bool _isServiceConnected;
        public bool IsServiceConnected
        {
            get { return _isServiceConnected; }
            set
            {
                _isServiceConnected = value;
                OnPropertyChanged("IsServiceConnected");
            }
        }

        bool _isDeviceConnected;
        public bool IsDeviceConnected
        {
            get { return _isDeviceConnected; }
            set
            {
                _isDeviceConnected = value;
                OnPropertyChanged("IsDeviceConnected");
            }
        }

        void OnConnectionLost()
        {
            IsServiceConnected = false;
        }

        void OnConnectionAppeared()
        {
            IsServiceConnected = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}