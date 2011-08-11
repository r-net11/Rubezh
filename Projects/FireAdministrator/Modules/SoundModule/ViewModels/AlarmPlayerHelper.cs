using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Media;
using Infrastructure.Common;
using System.Threading;
using FiresecAPI.Models;
using System.IO;

namespace SoundsModule.ViewModels
{
    public static class AlarmPlayerHelper
    {
        static AlarmPlayerHelper()
        {
            SoundPlr = new SoundPlayer();
        }

        static string CurrentDirectory
        {
            get { return Directory.GetCurrentDirectory() + @"\Sounds\"; }
        }

        static SoundPlayer SoundPlr { get; set; }

        static Thread thread { get; set; }

        static void PlayBeepContinious(object freq)
        {
            int frequency = (int)freq;
            while (true)
            {
                Console.Beep(frequency, 1000);
                Thread.Sleep(500);
            }
        }

        static void PlayBeep(object freq)
        {
            int frequency = (int)freq;
            Console.Beep(frequency, 1000);
        }

        static void StopPlayPCSpeaker()
        {
            if (thread != null)
            {
                thread.Abort();
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
                thread = new Thread(PlayBeepContinious);
                thread.Start((int)speaker);
            }
            else
            {
                thread = new Thread(PlayBeep);
                thread.Start((int)speaker);
            }
        }

        static void PlaySound(string SoundName, bool isContinious)
        {
            SoundPlr.SoundLocation = CurrentDirectory + SoundName;

            if (!string.IsNullOrWhiteSpace(SoundName))
            {
                SoundPlr.Load();
                if (SoundPlr.IsLoadCompleted)
                {
                    if (isContinious)
                    {
                        SoundPlr.PlayLooping();
                    }
                    else
                    {
                        SoundPlr.Play();
                    }
                }
            }
        }

        static void StopPlaySound()
        {
            SoundPlr.Stop();
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
