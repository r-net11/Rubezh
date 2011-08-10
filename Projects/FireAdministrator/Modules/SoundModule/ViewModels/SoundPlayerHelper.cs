using System;
using System.Media;
using System.Threading;
using FiresecAPI.Models;

namespace SoundsModule.ViewModels
{
    public static class SoundPlayerHelper
    {
        static SoundPlayerHelper()
        {
            _soundPlayer = new SoundPlayer();
        }

        static SoundPlayer _soundPlayer { get; set; }

        static Thread _thread { get; set; }

        static void PlayBeepContinious(object freq)
        {
            int frequency = (int) freq;
            while (true)
            {
                Console.Beep(frequency, 1000);
                Thread.Sleep(500);
            }
        }

        static void PlayBeep(object freq)
        {
            int frequency = (int) freq;
            Console.Beep(frequency, 1000);
        }

        static public void StopPlayPCSpeaker()
        {
            if (_thread != null)
            {
                _thread.Abort();
            }
        }

        public static void PlayPCSpeaker(SpeakerType speaker, bool isContinious)
        {
            if (speaker == SpeakerType.None)
            {
                return;
            }
            if (isContinious)
            {
                _thread = new Thread(PlayBeepContinious);
                _thread.Start((int)speaker);
            }
            else
            {
                _thread = new Thread(PlayBeep);
                _thread.Start((int)speaker);
            }
        }

        public static void PlaySound(string SoundName, bool isContinious)
        {
            _soundPlayer.SoundLocation = DownloadHelper.CurrentDirectory + SoundName;

            if (!(string.IsNullOrWhiteSpace(SoundName)) && (!string.Equals(SoundName, DownloadHelper.DefaultName)))
            {
                _soundPlayer.Load();
                if (_soundPlayer.IsLoadCompleted)
                {
                    if (isContinious)
                    {
                        _soundPlayer.PlayLooping();
                    }
                    else
                    {
                        _soundPlayer.Play();
                    }
                }
            }
        }

        public static void StopPlaySound()
        {
            _soundPlayer.Stop();
        }
    }
}