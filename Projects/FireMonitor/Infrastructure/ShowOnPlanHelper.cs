using System;
using System.Linq;
using StrazhAPI.GK;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using FiresecClient;
using Infrastructure.Events;
using Infrustructure.Plans.Events;

namespace Infrastructure
{
	public static class ShowOnPlanHelper
	{
		public static void ShowSKDDevice(SKDDevice device, Plan plan = null)
		{
			var element = plan == null ? null : plan.ElementSKDDevices.FirstOrDefault(item => item.DeviceUID == device.UID);
			if (plan == null || element == null)
				ServiceFactory.Events.GetEvent<ShowSKDDeviceOnPlanEvent>().Publish(device);
			else
				ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
		}
		public static bool CanShowSKDDevice(SKDDevice device)
		{
			if (device != null)
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
			if (zone != null)
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
			if (door != null)
				foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
					if (plan.ElementDoors.Any(x => (x.DoorUID != Guid.Empty) && (x.DoorUID == door.UID)))
						return true;
			return false;
		}

		public static void ShowCamera(Camera camera)
		{
			ServiceFactory.Events.GetEvent<ShowCameraOnPlanEvent>().Publish(camera);
		}
		public static bool CanShowCamera(Camera camera)
		{
			if (camera != null)
				foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				{
					foreach (var element in plan.ElementExtensions)
						if (element is ElementCamera && ((ElementCamera)element).CameraUID == camera.UID)
							return true;
				}
			return false;
		}
	}
}