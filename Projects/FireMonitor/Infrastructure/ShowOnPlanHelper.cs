using System;
using System.Linq;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.SKD;
using RubezhClient;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using System.Collections.Generic;
using RubezhAPI.Models.Layouts;

namespace Infrastructure
{
	public static class ShowOnPlanHelper
	{
		static Guid _layoutUID;
		public static Guid LayoutUID
		{
			get { return _layoutUID; }
			set
			{
				_layoutUID = value;
				;
			}
		}

		public List<Plan> GetPlans()
		{
			if(LayoutUID == Guid.Empty)
			{
				return ClientManager.PlansConfiguration.AllPlans;
			}
			else
			{
				var plans = new List<Guid>();
				var layout = ClientManager.LayoutsConfiguration.Layouts.FirstOrDefault(x => x.UID == LayoutUID);
				if(layout != null)
				{
					foreach(var part in layout.Parts)
					{
						if(part.Properties != null && part.Properties is LayoutPartPlansProperties)
						{
							var layoutPartPlansProperties = part.Properties as LayoutPartPlansProperties;
							foreach(var planUID in layoutPartPlansProperties.Plans)
							{
								if(!plans.Any(x=>x == planUID))
								{
									plans.Add(planUID);
								}
							}
						}
					}
				}
				return ClientManager.PlansConfiguration.AllPlans.Where(x => plans.Any(w => w == x.UID)).ToList();
			}
		}

		public static void ShowDevice(GKDevice device, Plan plan = null)
		{
			//var element = plan == null ? null : plan.ElementGKDevices.FirstOrDefault(item => item.DeviceUID == device.UID);
			//if (plan == null || element == null)
				//ServiceFactory.Events.GetEvent<ShowGKDeviceOnPlanEvent>().Publish(device);
			//else
				ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(Guid.Empty, device.UID));
		}
		public static bool CanShowDevice(GKDevice device)
		{
			if (device != null)
				foreach (var plan in ClientManager.PlansConfiguration.AllPlans)
					if (plan.ElementGKDevices.Any(x => x.DeviceUID == device.UID))
						return true;
			return false;
		}

		public static void ShowZone(GKZone zone)
		{
			ServiceFactory.Events.GetEvent<ShowGKZoneOnPlanEvent>().Publish(zone);
		}
		public static bool CanShowZone(GKZone zone)
		{
			if (zone != null)
				foreach (var plan in ClientManager.PlansConfiguration.AllPlans)
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
			ServiceFactory.Events.GetEvent<ShowGKGuardZoneOnPlanEvent>().Publish(zone);
		}
		public static bool CanShowGuardZone(GKGuardZone zone)
		{
			if (zone != null)
				foreach (var plan in ClientManager.PlansConfiguration.AllPlans)
				{
					if (plan.ElementPolygonGKGuardZones.Any(x => (x.ZoneUID != Guid.Empty) && (x.ZoneUID == zone.UID)))
						return true;
					if (plan.ElementRectangleGKGuardZones.Any(x => (x.ZoneUID != Guid.Empty) && (x.ZoneUID == zone.UID)))
						return true;
				}
			return false;
		}

		public static void ShowGKSKDZone(GKSKDZone zone)
		{
			ServiceFactory.Events.GetEvent<ShowGKSKDZoneOnPlanEvent>().Publish(zone);
		}
		public static bool CanShowGKSKDZone(GKSKDZone zone)
		{
			if (zone != null)
				foreach (var plan in ClientManager.PlansConfiguration.AllPlans)
				{
					if (plan.ElementPolygonGKSKDZones.Any(x => (x.ZoneUID != Guid.Empty) && (x.ZoneUID == zone.UID)))
						return true;
					if (plan.ElementRectangleGKSKDZones.Any(x => (x.ZoneUID != Guid.Empty) && (x.ZoneUID == zone.UID)))
						return true;
				}
			return false;
		}

		public static void ShowDelay(GKDelay delay)
		{
			ServiceFactory.Events.GetEvent<ShowGKDelayOnPlanEvent>().Publish(delay);
		}
		public static bool CanShowDelay(GKDelay delay)
		{
			if (delay != null)
			{
				foreach (var plan in ClientManager.PlansConfiguration.AllPlans)
				{
					if (plan.ElementRectangleGKDelays.Any(x => x.DelayUID == delay.UID))
						return true;
					if (plan.ElementPolygonGKDelays.Any(x => x.DelayUID == delay.UID))
						return true;
				}
			}
			return false;
		}

		public static void ShowDirection(GKDirection direction)
		{
			ServiceFactory.Events.GetEvent<ShowGKDirectionOnPlanEvent>().Publish(direction);
		}
		public static bool CanShowDirection(GKDirection direction)
		{
			if (direction != null)
				foreach (var plan in ClientManager.PlansConfiguration.AllPlans)
				{
					if (plan.ElementRectangleGKDirections.Any(x => x.DirectionUID == direction.UID))
						return true;
					if (plan.ElementPolygonGKDirections.Any(x => x.DirectionUID == direction.UID))
						return true;
				}
			return false;
		}

		public static void ShowMPT(GKMPT mpt)
		{
			ServiceFactory.Events.GetEvent<ShowGKMPTOnPlanEvent>().Publish(mpt);
		}
		public static bool CanShowMPT(GKMPT mpt)
		{
			if (mpt != null)
				foreach (var plan in ClientManager.PlansConfiguration.AllPlans)
				{
					if (plan.ElementRectangleGKMPTs.Any(x => x.MPTUID == mpt.UID))
						return true;
					if (plan.ElementPolygonGKMPTs.Any(x => x.MPTUID == mpt.UID))
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
			if (door != null)
				foreach (var plan in ClientManager.PlansConfiguration.AllPlans)
					if (plan.ElementGKDoors.Any(x => x.DoorUID != Guid.Empty && x.DoorUID == door.UID))
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
				foreach (var plan in ClientManager.PlansConfiguration.AllPlans)
				{
					foreach (var element in plan.ElementExtensions)
						if (element is ElementCamera && ((ElementCamera)element).CameraUID == camera.UID)
							return true;
				}
			return false;
		}
	}
}