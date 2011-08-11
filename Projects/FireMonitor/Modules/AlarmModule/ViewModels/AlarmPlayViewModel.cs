using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecClient;
using System.Windows;

namespace AlarmModule.ViewModels
{
    public class AlarmPlayViewModel
    {
        public AlarmPlayViewModel()
        {
            FiresecEventSubscriber.DeviceStateChangedEvent += new Action<string>(OnDeviceStateChanged);
        }

        public void PlayAlarm(StateType stateType)
        {
            if (SysConfSounds == null)
            {
                return;
            }
            foreach (var sound in SysConfSounds)
            {
                if (sound.StateType == stateType)
                {
                    AlarmPlayerHelper.Play(sound.SoundName, sound.SpeakerType, sound.IsContinious);
                }
            }
        }

        List<Sound> SysConfSounds
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
            PlayAlarm(currentDeviceState.StateType);
        }
    }
}
