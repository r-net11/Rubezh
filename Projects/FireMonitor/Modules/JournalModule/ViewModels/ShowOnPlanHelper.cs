using System;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Events;

namespace JournalModule.ViewModels
{
	public static class ShowOnPlanHelper
	{
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

		public static void ShowSKDDevice(SKDDevice device)
		{
			ServiceFactory.Events.GetEvent<ShowSKDDeviceOnPlanEvent>().Publish(device);
		}
		public static bool CanShowSKDDevice(SKDDevice device)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				if (plan.ElementSKDDevices.Any(x => x.DeviceUID == device.UID))
					return true;
			return false;
		}

		public static void ShowSKDZone(SKDZone zone)
		{
			ServiceFactory.Events.GetEvent<ShowSKDZoneOnPlanEvent>().Publish(zone);
		}
		public static bool CanShowSKDZone(SKDZone zone)
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

		public static void ShowSKDDoor(SKDDoor door)
		{
			ServiceFactory.Events.GetEvent<ShowSKDDoorOnPlanEvent>().Publish(door);
		}
		public static bool CanShowSKDDoor(SKDDoor door)
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