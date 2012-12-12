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
		public override void CreateViewModels()
        {
            ServiceFactory.Events.GetEvent<DevicesStateChangedEvent>().Unsubscribe(OnDevicesStateChanged);
            ServiceFactory.Events.GetEvent<DevicesStateChangedEvent>().Subscribe(OnDevicesStateChanged);
        }

		void OnDevicesStateChanged(object obj)
        {
            UpdateVideoAlarms();
        }

        void UpdateVideoAlarms()
        {
            foreach (var camera in FiresecManager.SystemConfiguration.Cameras)
            {
                foreach (var zoneUID in camera.ZoneUIDs)
                {
                    var zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
					if (zone != null)
                    {
						if (zone.ZoneState.StateType == camera.StateType)
                        {
                            VideoService.Show(camera);
                        }
                    }
                }
            }
        }

        public override void Initialize()
        {
            OnDevicesStateChanged(Guid.Empty);
        }
        public override IEnumerable<NavigationItem> CreateNavigation()
        {
            return new List<NavigationItem>()
            {
            };
        }

        public override void Dispose()
        {
            VideoService.Close();
        }
    }
}