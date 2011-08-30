using System;
using System.IO;
using System.Media;
using System.Threading;
using FiresecAPI.Models;

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
                Console.Beep(_frequency, 600);
                Thread.Sleep(1000);
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

        static void PlayPCSpeaker(BeeperType speaker, bool isContinious)
        {
            if (speaker == BeeperType.None)
            {
                return;
            }
            _frequency = (int)speaker;
            _isContinious = isContinious;

            if (_thread != null)
            {
                _thread.Abort();
            }
            _thread = new Thread(PlayBeep);
            _thread.Start();
        }

        static void PlaySound(string filePath, bool isContinious)
        {
            if (File.Exists(filePath))
            {
                _soundPlayer.SoundLocation = filePath;
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

        public static void Play(string filePath, BeeperType speakertype, bool isContinious)
        {
            PlaySound(filePath, isContinious);
            PlayPCSpeaker(speakertype, isContinious);
        }

        public static void Stop()
        {
            StopPlaySound();
            StopPlayPCSpeaker();
        }

        public static void Dispose()
        {
            Stop();
        }
    }
}
