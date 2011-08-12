using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;
using FiresecAPI.Models;
using Common;

namespace AlarmModule.ViewModels
{
    class AlarmPlayViewModel
    {
        public AlarmPlayViewModel()
        {
            FiresecEventSubscriber.DeviceStateChangedEvent += new Action<string>(OnDeviceStateChanged);
            _currentStateType = StateType.Norm;
            _isContinious = false;
        }

        StateType _currentStateType;
        StateType CurrentStateType
        {
            get { return _currentStateType; }
        }

        bool _isContinious;
        public bool IsContinious
        {
            get { return _isContinious; }
        }

        List<Sound> Sounds
        {
            get { return new List<Sound>(FiresecClient.FiresecManager.SystemConfiguration.Sounds); }
        }

        public void OnDeviceStateChanged(string deviceId)
        {
            var deviceStates = FiresecManager.DeviceStates.DeviceStates;
            var currentDeviceState = deviceStates[0];
            int minState = deviceStates[0].StateClassId;
            foreach (var deviceState in FiresecManager.DeviceStates.DeviceStates)
            {
                if (deviceState.StateClassId < minState)
                {
                    minState = deviceState.StateClassId;
                    currentDeviceState = deviceState;
                }
            }
            _currentStateType = currentDeviceState.StateType;
            PlayAlarm();
        }

        public void PlayAlarm()
        {
            if (Sounds == null)
            {
                return;
            }
            foreach (var sound in Sounds)
            {
                if (sound.StateType == CurrentStateType)
                {
                    _isContinious = sound.IsContinious;
                    string soundPath = FiresecManager.FileHelper.GetFilePath(sound.SoundName);
                    AlarmPlayerHelper.Play(soundPath, sound.SpeakerType, sound.IsContinious);
                    return;
                }
            }
        }

        public void StopPlayAlarm()
        {
            AlarmPlayerHelper.Stop();
        }
    }
}
