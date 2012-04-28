using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace FireMonitor.Views
{
    public partial class SoundView : UserControl, INotifyPropertyChanged
    {
        public SoundView()
        {
            InitializeComponent();
            DataContext = this;
            
            FiresecEventSubscriber.DeviceStateChangedEvent -= new Action<Guid>(OnDeviceStateChanged);
            FiresecEventSubscriber.DeviceStateChangedEvent += new Action<Guid>(OnDeviceStateChanged);
            PlaySoundCommand = new RelayCommand(OnPlaySound);
            CurrentStateType = StateType.No;
            IsSoundOn = true;
            IsEnabled = false;
            OnDeviceStateChanged(Guid.Empty);
        }

        public StateType CurrentStateType { get; private set; }

        bool _isSoundOn;
        public bool IsSoundOn
        {
            get { return _isSoundOn; }
            set
            {
                _isSoundOn = value;
                OnPropertyChanged("IsSoundOn");
            }
        }

        List<Sound> Sounds
        {
            get { return FiresecClient.FiresecManager.SystemConfiguration.Sounds; }
        }

        public void OnDeviceStateChanged(Guid deviceUID)
        {
            var minState = StateType.No;

            foreach (var deviceState in FiresecManager.DeviceStates.DeviceStates)
                if (deviceState.StateType < minState)
                    minState = deviceState.StateType;

            if (CurrentStateType != minState)
                CurrentStateType = minState;

            IsSoundOn = true;
            if (minState == StateType.Norm)
                IsEnabled = false;
            else
                IsEnabled = true;
            StopPlayAlarm();
            PlayAlarm();
        }

        public void PlayAlarm()
        {
            if (Sounds.IsNotNullOrEmpty() == false)
            {
                IsSoundOn = false;
                return;
            }
            foreach (var sound in Sounds)
            {
                if (sound.StateType == CurrentStateType)
                {
                    AlarmPlayerHelper.Play(FiresecClient.FileHelper.GetSoundFilePath(sound.SoundName), sound.BeeperType, sound.IsContinious);
                    return;
                }
            }
        }

        public void StopPlayAlarm()
        {
            AlarmPlayerHelper.Stop();
        }
        
        public RelayCommand PlaySoundCommand { get; private set; }
        void OnPlaySound()
        {
            if (IsSoundOn)
            {
                StopPlayAlarm();
                IsSoundOn = false;
            }
            else
            {
                PlayAlarm();
                IsSoundOn = true;
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