using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Media;
using System.IO;
using FiresecAPI.Models;
using System.Threading;

namespace AlarmModule.ViewModels
{
    public static class AlarmPlayerHelper
    {
        static AlarmPlayerHelper()
        {
            _soundPlayer = new SoundPlayer();
        }

        static string CurrentDirectory
        {
            get { return Directory.GetCurrentDirectory() + @"\Sounds\"; }
        }

        static SoundPlayer SoundPlr { get; set; }

        static Thread thread { get; set; }

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

        static void StopPlayPCSpeaker()
        {
            if (_thread != null)
            {
                _thread.Abort();
            }
        }

        static void PlayPCSpeaker(SpeakerType speaker, bool isContinious)
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

        static void PlaySound(string SoundName, bool isContinious)
        {
            SoundPlr.SoundLocation = CurrentDirectory + SoundName;

            if (!string.IsNullOrWhiteSpace(SoundName))
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

        static void StopPlaySound()
        {
            _soundPlayer.Stop();
        }

        public static void Play(string soundName, SpeakerType speakertype, bool isContinious)
        {
            PlaySound(soundName, isContinious);
            PlayPCSpeaker(speakertype, isContinious);
        }

        public static void Stop()
        {
            StopPlaySound();
            StopPlayPCSpeaker();
        }
    }
}
