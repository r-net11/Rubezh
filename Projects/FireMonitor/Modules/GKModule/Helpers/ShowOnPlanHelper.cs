using System;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Events;
using Infrustructure.Plans.Events;

namespace GKModule
{
	public static class ShowOnPlanHelper
	{
		public static void ShowDevice(GKDevice device, Plan plan)
		{
			var element = plan == null ? null : plan.ElementGKDevices.FirstOrDefault(item => item.DeviceUID == device.UID);
			if (plan == null || element == null)
				ShowDevice(device);
			else
				ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
		}
		public static void ShowDevice(GKDevice device)
		{
			ServiceFactory.Events.GetEvent<ShowXDeviceOnPlanEvent>().Publish(device);
		}
		public static bool CanShowDevice(GKDevice device)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				if (plan.ElementGKDevices.Any(x => x.DeviceUID == device.UID))
				{
					return true;
				}
			}
			return false;
		}

		public static void ShowZone(GKZone zone)
		{
			ServiceFactory.Events.GetEvent<ShowXZoneOnPlanEvent>().Publish(zone);
		}
		public static bool CanShowZone(GKZone zone)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				if (plan.ElementPolygonGKZones.Any(x => (x.ZoneUID != Guid.Empty) && (x.ZoneUID == zone.UID)))
					return true;
				if (plan.ElementRectangleGKZones.Any(x => (x.ZoneUID != Guid.Empty) && (x.ZoneUID == zone.UID)))
					return true;
			}
			return false;
		}

		public static void ShowGuardZone(GKGuardZone zone)
		{
			ServiceFactory.Events.GetEvent<ShowXGuardZoneOnPlanEvent>().Publish(zone);
		}
		public static bool CanShowGuardZone(GKGuardZone zone)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				if (plan.ElementPolygonGKGuardZones.Any(x => (x.ZoneUID != Guid.Empty) && (x.ZoneUID == zone.UID)))
					return true;
				if (plan.ElementRectangleGKGuardZones.Any(x => (x.ZoneUID != Guid.Empty) && (x.ZoneUID == zone.UID)))
					return true;
			}
			return false;
		}

		public static void ShowDirection(GKDirection direction)
		{
			ServiceFactory.Events.GetEvent<ShowXDirectionOnPlanEvent>().Publish(direction);
		}
		public static bool CanShowDirection(GKDirection direction)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				if (plan.ElementRectangleGKDirections.Any(x => x.DirectionUID == direction.UID))
					return true;
				if (plan.ElementPolygonGKDirections.Any(x => x.DirectionUID == direction.UID))
					return true;
			}
			return false;
		}

		public static void ShowDoor(GKDoor door)
		{
			ServiceFactory.Events.GetEvent<ShowGKDoorOnPlanEvent>().Publish(door);
		}
		public static bool CanShowDoor(GKDoor door)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				if (plan.ElementGKDoors.Any(x => x.DoorUID != Guid.Empty && x.DoorUID == door.UID))
					return true;
			}
			return false;
		}
	}
}