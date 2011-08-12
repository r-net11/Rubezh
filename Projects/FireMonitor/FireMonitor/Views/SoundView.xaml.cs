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

namespace FireMonitor
{
    public partial class SoundView : UserControl
    {
        public SoundView()
        {
            InitializeComponent();
            FiresecEventSubscriber.DeviceStateChangedEvent += new Action<string>(OnDeviceStateChanged);
            DataContext = this;
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
                    string soundPath = FiresecManager.FileHelper.GetFilePath(sound.SoundName);
                    AlarmPlayerHelper.Play(soundPath, sound.SpeakerType, sound.IsContinious);
                    return;
                }
            }
        }

        private void XButton_Checked(object sender, RoutedEventArgs e)
        {
            Image image = new Image();
            ImageSourceConverter convert = new ImageSourceConverter();
            ImageSource imageSource = (ImageSource)convert.ConvertFromString("Images/mute.png");
            image.Source = imageSource;
            SoundButton.Content = image;
            //SoundButton.Content = Image.SourceProperty("/FireMonitor;component/Images/mute.png");
        }
    }
}
