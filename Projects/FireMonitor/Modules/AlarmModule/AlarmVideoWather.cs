using System;
using System.Linq;
using FiresecClient;
using Infrastructure.Common;

namespace AlarmModule
{
	public class AlarmVideoWather
	{
		public AlarmVideoWather()
		{
			FiresecEventSubscriber.DeviceStateChangedEvent -= new Action<Guid>(OnDeviceStateChangedEvent);
			FiresecEventSubscriber.DeviceStateChangedEvent += new Action<Guid>(OnDeviceStateChangedEvent);
			OnDeviceStateChangedEvent(Guid.Empty);
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
	}
}