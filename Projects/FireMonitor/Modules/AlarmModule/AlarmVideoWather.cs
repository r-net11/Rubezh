using System;
using System.Linq;
using FiresecClient;
using Infrastructure.Common;
using System.Collections.Generic;
using Infrastructure.Common.Navigation;

namespace AlarmModule
{
	public class AlarmVideoWather : ModuleBase
	{
		public AlarmVideoWather()
		{
			FiresecEventSubscriber.DeviceStateChangedEvent -= new Action<Guid>(OnDeviceStateChangedEvent);
			FiresecEventSubscriber.DeviceStateChangedEvent += new Action<Guid>(OnDeviceStateChangedEvent);
		}

		void OnDeviceStateChangedEvent(Guid obj)
		{
			UpdateVideoAlarms();
		}

		void UpdateVideoAlarms()
		{
			foreach (var camera in FiresecManager.SystemConfiguration.Cameras)
			{
				foreach (var zoneNo in camera.Zones)
				{
					var zone = FiresecManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == zoneNo);
					if (zone != null)
					{
						if (zone.StateType == camera.StateType)
						{
							VideoService.Show(camera);
						}
					}
				}
			}
		}

		public override void Initialize()
		{
			OnDeviceStateChangedEvent(Guid.Empty);
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
			};
		}
	}
}