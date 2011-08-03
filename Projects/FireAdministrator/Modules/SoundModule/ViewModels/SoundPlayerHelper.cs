using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Media;
using Infrastructure.Common;

namespace SoundsModule.ViewModels
{
    public class SoundPlayerHelper : RegionViewModel
    {
        public SoundPlayerHelper()
        {
            SoundPlr = new SoundPlayer();
            IsNowPlaying = false;
        }

        SoundPlayer _soundPlr;
        public SoundPlayer SoundPlr
        {
            get { return _soundPlr; }
            set
            {
                _soundPlr = value;
            }
        }

        bool _isNowPlaying;
        public bool IsNowPlaying 
        {
            get { return _isNowPlaying; }
            set 
            {
                _isNowPlaying = value;
                OnPropertyChanged("IsNowPlaying");
            }
        }

        public void PlaySound(string soundName, bool isContinious)
        {
            if (IsNowPlaying)
            {
                Stop();
                Play(soundName, isContinious);
            }
            else
            {
                Stop();
            }
        }

        public void Play(string SoundName, bool isContinious)
        {
            SoundPlr.SoundLocation = DownloadHelper.CurrentDirectory + SoundName;

            if (!(string.IsNullOrWhiteSpace(SoundName))&&(!string.Equals(SoundName, DownloadHelper.DefaultName)))
            {
                try
                {
                    SoundPlr.Load();
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
                IsNowPlaying = false;
                return;
            }

            if (SoundPlr.IsLoadCompleted)
            {
                if (isContinious)
                {
                    SoundPlr.PlayLooping();
                }
                else
                {
                    SoundPlr.Play();
                    IsNowPlaying = false;
                }
            }
        }

        public void Stop()
        {
            SoundPlr.Stop();
        }

        public void PlayPCSpeaker(object isContinious, object speaker)
        {

        }
    }
}
