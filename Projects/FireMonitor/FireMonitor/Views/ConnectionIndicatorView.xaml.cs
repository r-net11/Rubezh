using System;
using System.Windows;
using System.Windows.Controls;
using FiresecClient;
using System.ComponentModel;

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

        void SetConnectionState(string state)
        {
            _textBlock.Text = state;
        }

        void OnConnectionLost()
        {
            IsServiceConnected = false;
            Dispatcher.Invoke(new Action<string>(SetConnectionState), "Потеря связи");
        }

        void OnConnectionAppeared()
        {
            IsServiceConnected = true;
            Dispatcher.Invoke(new Action<string>(SetConnectionState), "");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
