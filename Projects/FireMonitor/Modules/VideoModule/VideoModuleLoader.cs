using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;

namespace VideoModule
{
    public class VideoModuleLoader : ModuleBase
    {
        public VideoModuleLoader()
        {
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
                    var zoneState = FiresecManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == zoneNo);
                    if (zoneState != null)
                    {
                        if (zoneState.StateType == camera.StateType)
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