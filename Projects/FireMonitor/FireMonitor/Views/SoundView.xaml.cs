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
                ButtonContentChanged(value);
            }
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
            IsSoundOn = true;
            PlayAlarm();
        }

        public void PlayAlarm()
        {
            if (Sounds == null)
            {
                return;
            }
            //AlarmPlayerHelper.Play(@"H:\Rubezh\Projects\FireMonitor\FireMonitor\bin\Debug\Sounds\Sound1.wav", SpeakerType.None, true);
            //foreach (var sound in Sounds)
            //{
            //    if (sound.StateType == CurrentStateType)
            //    {
            //        string soundPath = FiresecManager.FileHelper.GetFilePath(sound.SoundName);
            //        AlarmPlayerHelper.Play(soundPath, sound.SpeakerType, sound.IsContinious);
            //        return;
            //    }
            //}
        }

        public void StopPlayAlarm()
        {
            AlarmPlayerHelper.Stop();
        }

        private void ButtonContentChanged(bool isNowPlaying)
        {
            if (IsSoundOn)
            {
                Image image = new Image();
                image.Source = new BitmapImage(new Uri("Images/sound.png", UriKind.Relative));
                SoundButton.Content = image;
            }
            else
            {
                Image image = new Image();
                image.Source = new BitmapImage(new Uri("Images/mute.png", UriKind.Relative));
                SoundButton.Content = image;
            }
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
            ButtonContentChanged(IsSoundOn);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
