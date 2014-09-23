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
		public static void ShowDevice(XDevice device)
		{
			ServiceFactory.Events.GetEvent<ShowXDeviceOnPlanEvent>().Publish(device);
		}
		public static bool CanShowDevice(XDevice device)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				if (plan.ElementXDevices.Any(x => x.XDeviceUID == device.UID))
				{
					return true;
				}
			}
			return false;
		}

		public static void ShowZone(XZone zone)
		{
			ServiceFactory.Events.GetEvent<ShowXZoneOnPlanEvent>().Publish(zone);
		}
		public static bool CanShowZone(XZone zone)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				if (plan.ElementPolygonXZones.Any(x => (x.ZoneUID != Guid.Empty) && (x.ZoneUID == zone.UID)))
					return true;
				if (plan.ElementRectangleXZones.Any(x => (x.ZoneUID != Guid.Empty) && (x.ZoneUID == zone.UID)))
					return true;
			}
			return false;
		}

		public static void ShowGuardZone(XGuardZone zone)
		{
			ServiceFactory.Events.GetEvent<ShowXGuardZoneOnPlanEvent>().Publish(zone);
		}
		public static bool CanShowGuardZone(XGuardZone zone)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				if (plan.ElementPolygonXGuardZones.Any(x => (x.ZoneUID != Guid.Empty) && (x.ZoneUID == zone.UID)))
					return true;
				if (plan.ElementRectangleXGuardZones.Any(x => (x.ZoneUID != Guid.Empty) && (x.ZoneUID == zone.UID)))
					return true;
			}
			return false;
		}

		public static void ShowDirection(XDirection direction)
		{
			ServiceFactory.Events.GetEvent<ShowXDirectionOnPlanEvent>().Publish(direction);
		}
		public static bool CanShowDirection(XDirection direction)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				if (plan.ElementRectangleXDirections.Any(x => x.DirectionUID == direction.UID))
					return true;
				if (plan.ElementPolygonXDirections.Any(x => x.DirectionUID == direction.UID))
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