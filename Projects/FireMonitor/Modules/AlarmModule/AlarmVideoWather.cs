using System;
using System.Linq;
using FiresecClient;
using Infrastructure.Common;
using System.Collections.Generic;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using Infrastructure;

namespace AlarmModule
{
	public class AlarmVideoWather : ModuleBase
	{
		public AlarmVideoWather()
		{
			//FiresecEventSubscriber.DeviceStateChangedEvent -= new Action<Guid>(OnDeviceStateChanged);
			//FiresecEventSubscriber.DeviceStateChangedEvent += new Action<Guid>(OnDeviceStateChanged);
			ServiceFactory.Events.GetEvent<DeviceStateChangedEvent>().Unsubscribe(OnDeviceStateChanged);
			ServiceFactory.Events.GetEvent<DeviceStateChangedEvent>().Subscribe(OnDeviceStateChanged);
		}

		void OnDeviceStateChanged(Guid obj)
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
			OnDeviceStateChanged(Guid.Empty);
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
			};
		}
	}
}