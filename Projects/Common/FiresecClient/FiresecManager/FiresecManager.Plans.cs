using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI.Models;

namespace FiresecClient
{
    public partial class FiresecManager
    {
        public static void InvalidatePlans()
        {
            try
            {
                foreach (var Plan in PlansConfiguration.AllPlans)
                {
                    var elementSubPlans = new List<ElementSubPlan>();
                    foreach (var elementSubPlan in Plan.ElementSubPlans)
                    {
                        var existingPlan = PlansConfiguration.AllPlans.FirstOrDefault(x => x.UID == elementSubPlan.PlanUID);
                        if (existingPlan != null)
                        {
                            elementSubPlans.Add(elementSubPlan);
                        }
                    }
                    Plan.ElementSubPlans = elementSubPlans;

                    var elementRectangleZones = new List<ElementRectangleZone>();
                    foreach (var elementRectangleZone in Plan.ElementRectangleZones.Where(x => x.ZoneUID != Guid.Empty))
                    {
                        var zone = Zones.FirstOrDefault(x => x.UID == elementRectangleZone.ZoneUID);
                        if (zone != null)
                        {
                            elementRectangleZones.Add(elementRectangleZone);
                        }
                    }
                    Plan.ElementRectangleZones = elementRectangleZones;

                    var elementPolygonZones = new List<ElementPolygonZone>();
                    foreach (var elementPolygonZone in Plan.ElementPolygonZones.Where(x => x.ZoneUID != Guid.Empty))
                    {
                        var zone = Zones.FirstOrDefault(x => x.UID == elementPolygonZone.ZoneUID);
                        if (zone != null)
                        {
                            elementPolygonZones.Add(elementPolygonZone);
                        }
                    }
                    Plan.ElementPolygonZones = elementPolygonZones;

                    var elementDevices = new List<ElementDevice>();
                    foreach (var elementDevice in Plan.ElementDevices)
                    {
                        if (elementDevice.DeviceUID == Guid.Empty)
                            continue;

                        var device = Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
                        if (device != null)
                        {
                            elementDevices.Add(elementDevice);
                        }
                    }
                    Plan.ElementDevices = elementDevices;

                    var elementRectangleXZones = new List<ElementRectangleXZone>();
                    foreach (var elementRectangleXZone in Plan.ElementRectangleXZones.Where(x => x.ZoneUID != Guid.Empty))
                    {
                        var zone = XManager.Zones.FirstOrDefault(x => x.UID == elementRectangleXZone.ZoneUID);
                        if (zone != null)
                        {
                            elementRectangleXZones.Add(elementRectangleXZone);
                        }
                    }
                    Plan.ElementRectangleXZones = elementRectangleXZones;

                    var elementPolygonXZones = new List<ElementPolygonXZone>();
                    foreach (var elementPolygonXZone in Plan.ElementPolygonXZones.Where(x => x.ZoneUID != Guid.Empty))
                    {
                        var zone = XManager.Zones.FirstOrDefault(x => x.UID == elementPolygonXZone.ZoneUID);
                        if (zone != null)
                        {
                            elementPolygonXZones.Add(elementPolygonXZone);
                        }
                    }
                    Plan.ElementPolygonXZones = elementPolygonXZones;

                    var elementXDevices = new List<ElementXDevice>();
                    foreach (var elementXDevice in Plan.ElementXDevices)
                    {
                        if (elementXDevice.XDeviceUID == Guid.Empty)
                            continue;

                        var device = XManager.Devices.FirstOrDefault(x => x.UID == elementXDevice.XDeviceUID);
                        if (device != null)
                        {
                            elementXDevices.Add(elementXDevice);
                        }
                    }
                    Plan.ElementXDevices = elementXDevices;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "FiresecManager.InvalidatePlans");
            }
        }
		public static bool PlanValidator(PlansConfiguration configuration)
		{
			//ServiceFactoryBase.ContentService.
			return true;
		}
    }
}