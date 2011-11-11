using System;
using System.Windows.Threading;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class DeviceControlViewModel : BaseViewModel
    {
        Device _device;

        public DeviceControlViewModel(Device device)
        {
            CloseCommand = new RelayCommand(OnClose);
            StopCommand = new RelayCommand(OnStop);
            OpenCommand = new RelayCommand(OnOpen);
            AutomaticOnCommand = new RelayCommand(OnAutomaticOn);
            AutomaticOffCommand = new RelayCommand(OnAutomaticOff);
            StartCommand = new RelayCommand(OnStart);
            CancelStartCommand = new RelayCommand(OnCancelStart);
            ConfirmCommand = new RelayCommand(OnConfirm);
            StopTimerCommand = new RelayCommand(OnStopTimer);

            _device = device;
        }

        public RelayCommand CloseCommand { get; private set; }
        void OnClose()
        {
            FiresecManager.ExecuteCommand(_device.UID, "BoltClose");
        }

        public RelayCommand StopCommand { get; private set; }
        void OnStop()
        {
            FiresecManager.ExecuteCommand(_device.UID, "BoltStop");
        }

        public RelayCommand OpenCommand { get; private set; }
        void OnOpen()
        {
            FiresecManager.ExecuteCommand(_device.UID, "BoltOpen");
        }

        public RelayCommand AutomaticOnCommand { get; private set; }
        void OnAutomaticOn()
        {
            FiresecManager.ExecuteCommand(_device.UID, "BoltAutoOn");
        }

        public RelayCommand AutomaticOffCommand { get; private set; }
        void OnAutomaticOff()
        {
            FiresecManager.ExecuteCommand(_device.UID, "BoltAutoOff");
        }

        public RelayCommand StartCommand { get; private set; }
        void OnStart()
        {
        }

        public RelayCommand CancelStartCommand { get; private set; }
        void OnCancelStart()
        {
        }

        public RelayCommand ConfirmCommand { get; private set; }
        void OnConfirm()
        {
        }

        bool _isTimerEnabled;
        public bool IsTimerEnabled
        {
            get { return _isTimerEnabled; }
            set
            {
                _isTimerEnabled = value;
                OnPropertyChanged("IsTimerEnabled");
            }
        }

        int _timeLeft;
        public int TimeLeft
        {
            get { return _timeLeft; }
            set
            {
                _timeLeft = value;
                OnPropertyChanged("TimeLeft");

                if (TimeLeft <= 0)
                    IsTimerEnabled = false;
            }
        }

        public void StartTimer(int timeLeft)
        {
            TimeLeft = timeLeft;
            IsTimerEnabled = true;
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Start();
        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            TimeLeft--;
        }

        public RelayCommand StopTimerCommand { get; private set; }
        void OnStopTimer()
        {
            TimeLeft = 0;
        }
    }
}