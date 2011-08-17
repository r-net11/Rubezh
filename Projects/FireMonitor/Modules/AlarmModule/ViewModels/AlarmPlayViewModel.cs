using System;
using System.Collections.Generic;
using Common;
using Infrastructure.Common;
using FiresecAPI.Models;
using FiresecClient;

namespace AlarmModule.ViewModels
{
    public class AlarmPlayViewModel
    {
        public AlarmPlayViewModel()
        {
            //FiresecEventSubscriber.DeviceStateChangedEvent += new Action<string>(OnDeviceStateChanged);
            //CurrentStateType = StateType.No;
            //IsSoundOn = true;
        }

        public StateType CurrentStateType { get; private set; }

        bool _isSoundOn;
        public bool IsSoundOn
        {
            get { return _isSoundOn; }
            set
            {
                _isSoundOn = value;
            }
        }

        static List<Sound> Sounds
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

        static public void StopPlayAlarm()
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