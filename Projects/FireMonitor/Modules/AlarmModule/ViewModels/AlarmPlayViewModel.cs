using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;
using FiresecAPI.Models;
using Common;
using Infrastructure.Common;

namespace AlarmModule.ViewModels
{
    class AlarmPlayViewModel
    {
        public AlarmPlayViewModel()
        {
            FiresecEventSubscriber.DeviceStateChangedEvent += new Action<string>(OnDeviceStateChanged);
            CurrentStateType = StateType.No;
            IsSoundOn = true;
            DataContext = this;
            OnDeviceStateChanged("");
            PlaySoundCommand = new RelayCommand(OnPlaySound);
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

        public void OnDeviceStateChanged(string deviceId)
        {
            var deviceStates = FiresecManager.DeviceStates.DeviceStates;
            var minState = StateType.No;
            foreach (var deviceState in FiresecManager.DeviceStates.DeviceStates)
            {
                if (deviceState.StateType < minState)
                {
                    minState = deviceState.StateType;
                }
            }
            if (CurrentStateType != minState)
            {
                CurrentStateType = minState;
                IsSoundOn = true;
            }
            PlayAlarm();
        }

        public void PlayAlarm()
        {
            if (Sounds == null)
            {
                IsSoundOn = false;
                return;
            }
            foreach (var sound in Sounds)
            {
                if (sound.StateType == CurrentStateType)
                {
                    string soundPath = FiresecManager.FileHelper.GetSoundFilePath(sound.SoundName);
                    AlarmPlayerHelper.Play(soundPath, sound.BeeperType, sound.IsContinious);
                    return;
                }
            }
        }

        public void StopPlayAlarm()
        {
            AlarmPlayerHelper.Stop();
        }

        //public RelayCommand PlaySoundCommand { get; private set; }
        //void OnPlaySound()
        //{
        //    if (IsSoundOn)
        //    {
        //        StopPlayAlarm();
        //        IsSoundOn = false;
        //    }
        //    else
        //    {
        //        PlayAlarm();
        //        IsSoundOn = true;
        //    }
        //}
    }
}
