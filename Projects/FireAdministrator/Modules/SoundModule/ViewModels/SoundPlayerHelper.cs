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

        static public void PlaySound(string SoundName, bool isContinious)
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

        static public void StopPlaySound()
        {
            SoundPlr.Stop();
        }

        static void PlayBeepContinious(object isContinious)
        {
            bool IsContinious = (bool)isContinious;
            if (IsContinious)
            {
                while (true)
                {
                    Console.Beep(1000, 1000);
                    Thread.Sleep(500);
                }
            }
            else
            {
                Console.Beep(1000, 1000);
            }
        }


        static public void PlayPCSpeaker(string Speaker, bool isContinious)
        {
            if (!(string.IsNullOrWhiteSpace(Speaker)) && (!string.Equals(Speaker, DownloadHelper.DefaultName)))
            {
                thread = new Thread(PlayBeepContinious);
                thread.Start(isContinious);
            }
        }

        static public void StopPlayPCSpeaker()
        {
            thread.Abort();
        }
    }
}
