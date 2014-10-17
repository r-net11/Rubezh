using System;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure.Events;
using Infrustructure.Plans.Events;

namespace Infrastructure
{
    public static class ShowOnPlanHelper
    {
        public static void ShowDevice(GKDevice device, Plan plan = null)
        {
            var element = plan == null ? null : plan.ElementGKDevices.FirstOrDefault(item => item.DeviceUID == device.UID);
            if (plan == null || element == null)
                ServiceFactory.Events.GetEvent<ShowXDeviceOnPlanEvent>().Publish(device);
            else
                ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
        }
        public static bool CanShowDevice(GKDevice device)
        {
            if (device != null)
                foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
                    if (plan.ElementGKDevices.Any(x => x.DeviceUID == device.UID))
                        return true;
            return false;
        }

        public static void ShowZone(GKZone zone)
        {
            ServiceFactory.Events.GetEvent<ShowXZoneOnPlanEvent>().Publish(zone);
        }
        public static bool CanShowZone(GKZone zone)
        {
            if (zone != null)
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
            if (zone != null)
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
            if (direction != null)
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
            if (door != null)
                foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
                    if (plan.ElementGKDoors.Any(x => x.DoorUID != Guid.Empty && x.DoorUID == door.UID))
                        return true;
            return false;
        }

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