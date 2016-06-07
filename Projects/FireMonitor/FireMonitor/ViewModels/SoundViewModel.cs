using System.Collections.Generic;
using StrazhAPI;
using StrazhAPI.Events;
using StrazhAPI.GK;
using StrazhAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using StrazhAPI.SKD;

namespace FireMonitor.ViewModels
{
	public class SoundViewModel : BaseViewModel
	{
		public SoundViewModel()
		{
			ServiceFactory.Events.GetEvent<SKDObjectsStateChangedEvent>().Unsubscribe(OnStateChanged);
			ServiceFactory.Events.GetEvent<SKDObjectsStateChangedEvent>().Subscribe(OnStateChanged);

			PlaySoundCommand = new RelayCommand(OnPlaySound);
			OnStateChanged(null);
		}

		bool _isSoundOn;
		public bool IsSoundOn
		{
			get { return _isSoundOn; }
			set
			{
				_isSoundOn = value;
				OnPropertyChanged(() => IsSoundOn);
			}
		}

		List<Sound> Sounds
		{
			get { return FiresecClient.FiresecManager.SystemConfiguration.Sounds; }
		}

		public void OnStateChanged(object obj)
		{
			IsSoundOn = true;
			PlayAlarm();
		}

		void PlayAlarm()
		{
			if (Sounds.IsNotNullOrEmpty() == false)
			{
				IsSoundOn = false;
				return;
			}

			var minSoundStateClass = XStateClass.Norm;
			Sound minSound = null;

			foreach (var sound in Sounds)
			{
				if (!string.IsNullOrEmpty(sound.SoundName))
				{
					var hasStateClass = false;

					foreach (var device in SKDManager.Devices)
					{
						if (device.State != null && device.State.StateClass == sound.StateClass)
						{
							hasStateClass = true;
							break;
						}
					}

					if (hasStateClass)
					{
						if (sound.StateClass < minSoundStateClass)
						{
							minSoundStateClass = sound.StateClass;
							minSound = sound;
						}
					}
				}
			}
			if (minSound != null)
			{
				AlarmPlayerHelper.Play(FiresecClient.FileHelper.GetSoundFilePath(minSound.SoundName), minSound.IsContinious);
			}
		}

		public RelayCommand PlaySoundCommand { get; private set; }
		void OnPlaySound()
		{
			if (IsSoundOn)
			{
				AlarmPlayerHelper.Stop();
				IsSoundOn = false;
			}
			else
			{
				PlayAlarm();
				IsSoundOn = true;
			}
		}
	}
}