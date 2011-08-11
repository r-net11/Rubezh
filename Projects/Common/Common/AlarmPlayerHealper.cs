using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using System.Threading;
using System.Media;
using System.IO;

namespace Common
{
    public static class AlarmPlayerHelper
    {
        static AlarmPlayerHelper()
        {
            _soundPlayer = new SoundPlayer();
        }

        static SoundPlayer _soundPlayer;

        static Thread _thread;

        static int _frequency;

        static bool _isContinious;

        static void PlayBeep()
        {
            do
            {
                Console.Beep(_frequency, 1000);
                Thread.Sleep(500);
            }
            while (_isContinious);
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
            _frequency = (int)speaker;
            _isContinious = isContinious;
            _thread = new Thread(PlayBeep);
            _thread.Start();
        }

        static void PlaySound(string filePath, bool isContinious)
        {
            _soundPlayer.SoundLocation = filePath;

            if (!string.IsNullOrWhiteSpace(filePath))
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

        public static void Play(string filePath, SpeakerType speakertype, bool isContinious)
        {
            PlaySound(filePath, isContinious);
            PlayPCSpeaker(speakertype, isContinious);
        }

        public static void Stop()
        {
            StopPlaySound();
            StopPlayPCSpeaker();
        }
    }
}
