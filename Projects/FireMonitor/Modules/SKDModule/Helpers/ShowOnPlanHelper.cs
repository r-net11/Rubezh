using System.Linq;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrustructure.Plans.Events;
using SKDModule.Events;
using Infrastructure.Events;
using System;

namespace SKDModule
{
	public static class ShowOnPlanHelper
	{
		public static void ShowDevice(SKDDevice device, Plan plan)
		{
			var element = plan == null ? null : plan.ElementSKDDevices.FirstOrDefault(item => item.DeviceUID == device.UID);
			if (plan == null || element == null)
				ServiceFactory.OnPublishEvent<SKDDevice, ShowSKDDeviceOnPlanEvent>(device);
			else
				ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
		}
		public static bool CanShowDevice(SKDDevice device)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				if (plan.ElementSKDDevices.Any(x => x.DeviceUID == device.UID))
					return true;
			return false;
		}

		public static void ShowZone(SKDZone zone)
		{
			ServiceFactory.Events.GetEvent<ShowSKDZoneOnPlanEvent>().Publish(zone);
		}
		public static bool CanShowZone(SKDZone zone)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				if (plan.ElementPolygonSKDZones.Any(x => (x.ZoneUID != Guid.Empty) && (x.ZoneUID == zone.UID)))
					return true;
				if (plan.ElementRectangleSKDZones.Any(x => (x.ZoneUID != Guid.Empty) && (x.ZoneUID == zone.UID)))
					return true;
			}
			return false;
		}

		public static void ShowDoor(Door door)
		{
			ServiceFactory.Events.GetEvent<ShowDoorOnPlanEvent>().Publish(door);
		}
		public static bool CanShowDoor(Door door)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				if (plan.ElementDoors.Any(x => (x.DoorUID != Guid.Empty) && (x.DoorUID == door.UID)))
					return true;
			}
			return false;
		}
	}
}