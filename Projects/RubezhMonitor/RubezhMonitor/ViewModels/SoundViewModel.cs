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
					switch (sound.Type)
					{
						case SoundType.Alarm:
							if (GKManager.GuardZones.Any(x => x.State != null && x.State.StateClass == sound.StateClass))
							{
								hasStateClass = true;
							}
							break;
						case SoundType.Fire1:
						case SoundType.Fire2:
						case SoundType.Attention:
							if (GKManager.Zones.Any(x => x.State != null && x.State.StateClass == sound.StateClass))
							{
								hasStateClass = true;
							}
							break;
						case SoundType.Failure:
							foreach (var device in GKManager.Devices.Where(x => x.IsRealDevice))
							{
								if (device.State.StateClass == sound.StateClass)
								{
									hasStateClass = true;
									break;
								}
							}
							break;
						case SoundType.Off:
							hasStateClass = CheckStatusMptNsNpt(sound.StateClass);
							foreach (var device in GKManager.Devices.Where(x => x.IsRealDevice))
							{
								if (device.State.StateClass == sound.StateClass)
								{
									hasStateClass = true;
									break;
								}
							}
							if (GKManager.Zones.Any(x => x.State != null && x.State.StateClass == sound.StateClass))
							{
								hasStateClass = true;
							}
							if (GKManager.GuardZones.Any(x => x.State != null && x.State.StateClass == sound.StateClass))
							{
								hasStateClass = true;
							}
							break;
						case SoundType.TurningOn:
							hasStateClass = CheckStatusMptNsNpt(sound.StateClass);
							break;
						case SoundType.StopStart:
							if ((GKManager.MPTs.Any(x => x.State != null && x.State.StateClass != XStateClass.On && x.State.StateClass != XStateClass.Off && x.State.StateClass != XStateClass.TurningOff 
							&& x.State.StateClass != XStateClass.TurningOn)))
							{
								hasStateClass = true;
							}
							if ((GKManager.PumpStations.Any(x => x.State != null && x.State.StateClass != XStateClass.On && x.State.StateClass != XStateClass.Off && x.State.StateClass != XStateClass.TurningOff
							&& x.State.StateClass != XStateClass.TurningOn)))
							{
								hasStateClass = true;
							}
							if ((GKManager.Directions.Any(x => x.State != null && x.State.StateClass != XStateClass.On && x.State.StateClass != XStateClass.Off && x.State.StateClass != XStateClass.TurningOff
							&& x.State.StateClass != XStateClass.TurningOn)))
							{
								hasStateClass = true;
							}
							break;
						case SoundType.AutoOff:
							hasStateClass = CheckStatusMptNsNpt(sound.StateClass);
							foreach (var device in GKManager.Devices.Where(x => x.IsRealDevice))
							{
								if (device.State.StateClass == sound.StateClass)
								{
									hasStateClass = true;
									break;
								}
							}
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
				AlarmPlayerHelper.Play(RubezhClient.FileHelper.GetSoundFilePath(minSound.SoundName), minSound.IsContinious);
			}
			else
			{
				AlarmPlayerHelper.Stop();
			}
		}

		bool CheckStatusMptNsNpt(XStateClass stateClass)
		{
			if ((GKManager.MPTs.Any(x => x.State != null && x.State.StateClass == stateClass)) ||
			(GKManager.PumpStations.Any(x => x.State != null && x.State.StateClass == stateClass)) ||
			(GKManager.Directions.Any(x => x.State != null && x.State.StateClass == stateClass)))
				return true;
			return false;
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