using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Media;
using Infrastructure.Common;
using System.Threading;

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

        //static void PlayBeepContinious(object isContinious)
        //{
        //    bool IsContinious = (bool)isContinious;
        //    if (IsContinious)
        //    {
        //        while (true)
        //        {
        //            Console.Beep(1000, 1000);
        //            Thread.Sleep(500);
        //        }
        //    }
        //    else
        //    {
        //        Console.Beep(1000, 1000);
        //    }
        //}

        static void PlayBeepContinious(object freq)
        {
            int frequency = (int)freq;
            while (true)
            {
                Console.Beep(frequency, 1000);
                Thread.Sleep(500);
            }
        }

        static public void StopPlayPCSpeaker()
        {
            thread.Abort();
        }

        static void PlayBeep(object freq)
        {
            int frequency = (int)freq;
            Console.Beep(frequency, 1000);
        }

        public static void PlayPCSpeaker(string Speaker, bool isContinious)
        {
            if (!(string.IsNullOrWhiteSpace(Speaker)) && (!string.Equals(Speaker, DownloadHelper.DefaultName)))
            {
                foreach (var id in Enum.GetValues(typeof(DownloadHelper.AvailableSpeaker)))
                {
                    if (id.ToString() == Speaker)
                    {
                        if (isContinious)
                        {
                            thread = new Thread(PlayBeepContinious);
                            thread.Start((int)id);
                        }
                        else
                        {
                            thread = new Thread(PlayBeep);
                            thread.Start((int)id);
                        }
                    }
                }
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
