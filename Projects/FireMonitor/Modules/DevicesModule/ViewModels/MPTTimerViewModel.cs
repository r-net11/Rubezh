using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Windows.Threading;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models;

namespace DevicesModule.ViewModels
{
    public class MPTTimerViewModel : DialogViewModel, IWindowIdentity
    {
        Device Device;
        private Guid _guid;

        public MPTTimerViewModel(Device device)
        {
			Title = "Включение: " +  device.PresentationAddressAndDriver;
            Device = device;
            _guid = device.UID;
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
                {
                    IsTimerEnabled = false;
                    Close();
                }
            }
        }

        public void StartTimer(int timeLeft)
        {
            TimeLeft = timeLeft;
            IsTimerEnabled = true;
            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Start();
        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            --TimeLeft;
        }

        public RelayCommand StopTimerCommand { get; private set; }
        void OnStopTimer()
        {
            TimeLeft = 0;
        }

        #region IWindowIdentity Members
        public string Guid
        {
            get { return "MPT_" + _guid.ToString(); }
        }
        #endregion
    }
}