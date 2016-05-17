using System.Collections.Generic;
using System.Linq;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using RubezhAPI.SKD;

namespace RubezhMonitor.ViewModels
{
	public class SoundViewModel : BaseViewModel
	{
		public SoundViewModel()
		{
			ServiceFactory.Events.GetEvent<GKObjectsStateChangedEvent>().Unsubscribe(OnStateChanged);
			ServiceFactory.Events.GetEvent<GKObjectsStateChangedEvent>().Subscribe(OnStateChanged);

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
			get { return ClientManager.SystemConfiguration.Sounds; }
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
					foreach (var device in GKManager.Devices.Where(x => x.IsRealDevice))
					{
						if (sound.StateClass != XStateClass.Attention && sound.StateClass != XStateClass.Fire1 && sound.StateClass != XStateClass.Fire2)
						{
							if (device.State.StateClass == sound.StateClass)
							{
								hasStateClass = true;
								break;
							}
						}
					}
					if (GKManager.Zones.Any(x => x.State != null && x.State.StateClass == sound.StateClass))
					{
						hasStateClass = true;
					}
					if (GKManager.Directions.Any(x => x.State != null && x.State.StateClass == sound.StateClass))
					{
						hasStateClass = true;
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
				AlarmPlayerHelper.Play(RubezhClient.FileHelper.GetSoundFilePath(minSound.SoundName), minSound.BeeperType, minSound.IsContinious);
			}
			else
			{
				AlarmPlayerHelper.Stop();
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