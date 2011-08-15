using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FiresecClient;
using FiresecAPI.Models;
using Common;
using Infrastructure.Common;
using System.ComponentModel;

namespace FireMonitor
{
    public partial class SoundView : UserControl, INotifyPropertyChanged
    {
        public SoundView()
        {
            InitializeComponent();
            FiresecEventSubscriber.DeviceStateChangedEvent += new Action<string>(OnDeviceStateChanged);
            _currentStateType = StateType.Norm;
            IsSoundOn = true;
            DataContext = this;
            PlaySoundCommand = new RelayCommand(OnPlaySound);
        }
        
        StateType _currentStateType;
        StateType CurrentStateType
        {
            get { return _currentStateType; }
        }

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
            get { return new List<Sound>(FiresecClient.FiresecManager.SystemConfiguration.Sounds); }
        }

        public void OnDeviceStateChanged(string deviceId)
        {
            var deviceStates = FiresecManager.DeviceStates.DeviceStates;
            var minState = StateType.Unknown;
            foreach (var deviceState in FiresecManager.DeviceStates.DeviceStates)
            {
                if (deviceState.StateType < minState)
                {
                    minState = deviceState.StateType;
                }
            }
            _currentStateType = minState;
            IsSoundOn = true;
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
                    AlarmPlayerHelper.Play(soundPath, sound.SpeakerType, sound.IsContinious);
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
