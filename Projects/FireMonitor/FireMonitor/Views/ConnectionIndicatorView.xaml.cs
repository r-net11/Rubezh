using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FiresecClient;
using FiresecAPI.Models;

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

            OnDeviceStateChangedEvent(null);
            FiresecEventSubscriber.DeviceStateChangedEvent += new Action<string>(OnDeviceStateChangedEvent);
        }

        void OnDeviceStateChangedEvent(string obj)
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