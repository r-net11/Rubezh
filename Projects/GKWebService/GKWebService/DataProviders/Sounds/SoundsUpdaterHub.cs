using GKWebService.Models.Sound;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RubezhAPI;
using RubezhAPI.GK;
using Microsoft.AspNet.SignalR.Hubs;

namespace GKWebService.DataProviders.Sounds
{
	[HubName("soundsUpdater")]
	public class SoundsUpdaterHub : Hub
	{
		public static SoundsUpdaterHub Instance { get; private set; }

		List<RubezhAPI.Models.Sound> Sounds
		{
			get { return RubezhClient.ClientManager.SystemConfiguration.Sounds; }
		}

		public SoundsUpdaterHub()
		{
			Instance = this;
		}

		public void BroadcastSounds()
		{
			if (Sounds.IsNotNullOrEmpty() == false)
			{
				//IsSoundOn = false;
				return;
			}

			var minSoundStateClass = XStateClass.Norm;
			RubezhAPI.Models.Sound minSound = null;

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
			//if (minSound != null)
			//{
			//	AlarmPlayerHelper.Play(RubezhClient.FileHelper.GetSoundFilePath(minSound.SoundName), minSound.BeeperType, minSound.IsContinious);
			//}
			//else
			//{
			//	AlarmPlayerHelper.Stop();
			//}

			Clients.All.updateSounds(new { sound = minSound });
		}
	}
}