using System;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using XFiresecAPI;

namespace GKModule
{
	public static class ShowOnPlanHelper
	{
		public static void ShowDevice(XDevice device, Plan plan)
		{
			var element = plan == null ? null : plan.ElementXDevices.FirstOrDefault(item => item.XDeviceUID == device.UID);
			if (plan == null || element == null)
				ShowDevice(device);
			else
				ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
		}
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
	}
}