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
            SoundPlr = new SoundPlayer();
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

        static public void StopPlayPCSpeaker()
        {
            if (thread != null)
            {
                thread.Abort();
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
                thread = new Thread(PlayBeepContinious);
                thread.Start((int)speaker);
            }
            else
            {
                thread = new Thread(PlayBeep);
                thread.Start((int)speaker);
            }
        }

        public static void PlaySound(string SoundName, bool isContinious)
        {
            SoundPlr.SoundLocation = DownloadHelper.CurrentDirectory + SoundName;

            if (!(string.IsNullOrWhiteSpace(SoundName)) && (!string.Equals(SoundName, DownloadHelper.DefaultName)))
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

        public static void StopPlaySound()
        {
            SoundPlr.Stop();
        }
    }
}