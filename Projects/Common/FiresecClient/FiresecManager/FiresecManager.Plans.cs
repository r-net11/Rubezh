using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using Infrustructure.Plans.Elements;

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
					var keys = PlansConfiguration.AllPlans.Select(item => item.UID).ToList();
					var elementSubPlans = new List<ElementSubPlan>();
					foreach (var elementSubPlan in Plan.ElementSubPlans)
						if (keys.Contains(elementSubPlan.PlanUID))
							elementSubPlans.Add(elementSubPlan);
					Plan.ElementSubPlans = elementSubPlans;

					keys = Zones.Select(item => item.UID).ToList();
					var elementRectangleZones = new List<ElementRectangleZone>();
					foreach (var elementRectangleZone in Plan.ElementRectangleZones.Where(x => x.ZoneUID != Guid.Empty))
						if (keys.Contains(elementRectangleZone.ZoneUID))
							elementRectangleZones.Add(elementRectangleZone);
					Plan.ElementRectangleZones = elementRectangleZones;

					var elementPolygonZones = new List<ElementPolygonZone>();
					foreach (var elementPolygonZone in Plan.ElementPolygonZones.Where(x => x.ZoneUID != Guid.Empty))
						if (keys.Contains(elementPolygonZone.ZoneUID))
							elementPolygonZones.Add(elementPolygonZone);
					Plan.ElementPolygonZones = elementPolygonZones;

					keys = Devices.Select(item => item.UID).ToList();
					var elementDevices = new List<ElementDevice>();
					foreach (var elementDevice in Plan.ElementDevices.Where(x => x.DeviceUID != Guid.Empty))
						if (keys.Contains(elementDevice.DeviceUID))
							elementDevices.Add(elementDevice);
					Plan.ElementDevices = elementDevices;

					keys = XManager.Zones.Select(item => item.BaseUID).ToList();
					var elementRectangleXZones = new List<ElementRectangleXZone>();
					foreach (var elementRectangleXZone in Plan.ElementRectangleXZones.Where(x => x.ZoneUID != Guid.Empty))
						if (keys.Contains(elementRectangleXZone.ZoneUID))
							elementRectangleXZones.Add(elementRectangleXZone);
					Plan.ElementRectangleXZones = elementRectangleXZones;

					var elementPolygonXZones = new List<ElementPolygonXZone>();
					foreach (var elementPolygonXZone in Plan.ElementPolygonXZones.Where(x => x.ZoneUID != Guid.Empty))
						if (keys.Contains(elementPolygonXZone.ZoneUID))
							elementPolygonXZones.Add(elementPolygonXZone);
					Plan.ElementPolygonXZones = elementPolygonXZones;

					keys = XManager.DeviceConfiguration.GuardZones.Select(item => item.BaseUID).ToList();
					var elementRectangleXGuardZones = new List<ElementRectangleXGuardZone>();
					foreach (var elementRectangleXGuardZone in Plan.ElementRectangleXGuardZones.Where(x => x.ZoneUID != Guid.Empty))
						if (keys.Contains(elementRectangleXGuardZone.ZoneUID))
							elementRectangleXGuardZones.Add(elementRectangleXGuardZone);
					Plan.ElementRectangleXGuardZones = elementRectangleXGuardZones;

					var elementPolygonXGuardZones = new List<ElementPolygonXGuardZone>();
					foreach (var elementPolygonXGuardZone in Plan.ElementPolygonXGuardZones.Where(x => x.ZoneUID != Guid.Empty))
						if (keys.Contains(elementPolygonXGuardZone.ZoneUID))
							elementPolygonXGuardZones.Add(elementPolygonXGuardZone);
					Plan.ElementPolygonXGuardZones = elementPolygonXGuardZones;

					keys = XManager.Devices.Select(item => item.BaseUID).ToList();
					var elementXDevices = new List<ElementXDevice>();
					foreach (var elementXDevice in Plan.ElementXDevices.Where(x => x.XDeviceUID != Guid.Empty))
						if (keys.Contains(elementXDevice.XDeviceUID))
							elementXDevices.Add(elementXDevice);
					Plan.ElementXDevices = elementXDevices;

					keys = SKDManager.Zones.Select(item => item.UID).ToList();
					var elementRectangleSKDZones = new List<ElementRectangleSKDZone>();
					foreach (var elementRectangleSKDZone in Plan.ElementRectangleSKDZones.Where(x => x.ZoneUID != Guid.Empty))
						if (keys.Contains(elementRectangleSKDZone.ZoneUID))
							elementRectangleSKDZones.Add(elementRectangleSKDZone);
					Plan.ElementRectangleSKDZones = elementRectangleSKDZones;

					var elementPolygonSKDZones = new List<ElementPolygonSKDZone>();
					foreach (var elementPolygonSKDZone in Plan.ElementPolygonSKDZones.Where(x => x.ZoneUID != Guid.Empty))
						if (keys.Contains(elementPolygonSKDZone.ZoneUID))
							elementPolygonSKDZones.Add(elementPolygonSKDZone);
					Plan.ElementPolygonSKDZones = elementPolygonSKDZones;

					keys = SKDManager.Devices.Select(item => item.UID).ToList();
					var elementSKDDevices = new List<ElementSKDDevice>();
					foreach (var elementSKDDevice in Plan.ElementSKDDevices.Where(x => x.DeviceUID != Guid.Empty))
						if (keys.Contains(elementSKDDevice.DeviceUID))
							elementSKDDevices.Add(elementSKDDevice);
					Plan.ElementSKDDevices = elementSKDDevices;

					keys = SKDManager.SKDConfiguration.Doors.Select(item => item.UID).ToList();
					var elementDoors = new List<ElementDoor>();
					foreach (var elementDoor in Plan.ElementDoors.Where(x => x.DoorUID != Guid.Empty))
						if (keys.Contains(elementDoor.DoorUID))
							elementDoors.Add(elementDoor);
					Plan.ElementDoors = elementDoors;

					keys = XManager.Devices.Select(item => item.BaseUID).ToList();
					var cameraKeys = SystemConfiguration.AllCameras.Select(item => item.UID).ToList();
					var elementExtensions = new List<ElementBase>();
					foreach (var elementExtension in Plan.ElementExtensions)
					{
						var elementTank = elementExtension as ElementRectangleTank;
						if (elementTank != null)
						{
							if (keys.Contains(elementTank.XDeviceUID))
								elementExtensions.Add(elementTank);
						}
						else
						{
							var elementCamera = elementExtension as ElementCamera;
							if (elementCamera != null)
							{
								if (cameraKeys.Contains(elementCamera.CameraUID))
									elementExtensions.Add(elementCamera);
							}
						}
					}
					Plan.ElementExtensions = elementExtensions;
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