using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace FireMonitor.ViewModels
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
					foreach (var device in GKManager.Devices)
						if (device.IsRealDevice)
						{
							var stateClass = device.State.StateClass;
							if (device.DriverType == GKDriverType.AM1_T && stateClass == XStateClass.Fire2)
								stateClass = XStateClass.Info;
							if (sound.StateClass != XStateClass.Attention && sound.StateClass != XStateClass.Fire1 && sound.StateClass != XStateClass.Fire2)
							{
								if (stateClass == sound.StateClass)
								{
									hasStateClass = true;
									break;
								}
							}
						}
					foreach (var zone in GKManager.Zones)
						if (zone.State != null && zone.State.StateClass == sound.StateClass)
						{
							hasStateClass = true;
							break;
						}
					foreach (var direction in GKManager.Directions)
						if (direction.State != null && direction.State.StateClass == sound.StateClass)
						{
							hasStateClass = true;
							break;
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
				AlarmPlayerHelper.Play(FiresecClient.FileHelper.GetSoundFilePath(minSound.SoundName), minSound.BeeperType, minSound.IsContinious);
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