using System;
using FiresecAPI.Models;
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
            foreach (var zoneState in FiresecManager.DeviceStates.ZoneStates)
            {
                if (zoneState.StateType == StateType.Fire)
                {
                    foreach (var camera in FiresecManager.SystemConfiguration.Cameras)
                    {
                        if (camera.Zones.Contains(zoneState.No))
                        {
                            VideoService.Show(camera.Address);
                        }
                    }
                }
            }
        }
    }
}