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

					keys = GKManager.Zones.Select(item => item.UID).ToList();
					var elementRectangleGKZones = new List<ElementRectangleGKZone>();
					foreach (var elementRectangleGKZone in Plan.ElementRectangleGKZones.Where(x => x.ZoneUID != Guid.Empty))
						if (keys.Contains(elementRectangleGKZone.ZoneUID))
							elementRectangleGKZones.Add(elementRectangleGKZone);
					Plan.ElementRectangleGKZones = elementRectangleGKZones;

					var elementPolygonGKZones = new List<ElementPolygonGKZone>();
					foreach (var elementPolygonGKZone in Plan.ElementPolygonGKZones.Where(x => x.ZoneUID != Guid.Empty))
						if (keys.Contains(elementPolygonGKZone.ZoneUID))
							elementPolygonGKZones.Add(elementPolygonGKZone);
					Plan.ElementPolygonGKZones = elementPolygonGKZones;

					keys = GKManager.DeviceConfiguration.GuardZones.Select(item => item.UID).ToList();
					var elementRectangleGKGuardZones = new List<ElementRectangleGKGuardZone>();
					foreach (var elementRectangleGKGuardZone in Plan.ElementRectangleGKGuardZones.Where(x => x.ZoneUID != Guid.Empty))
						if (keys.Contains(elementRectangleGKGuardZone.ZoneUID))
							elementRectangleGKGuardZones.Add(elementRectangleGKGuardZone);
					Plan.ElementRectangleGKGuardZones = elementRectangleGKGuardZones;

					var elementPolygonGKGuardZones = new List<ElementPolygonGKGuardZone>();
					foreach (var elementPolygonGKGuardZone in Plan.ElementPolygonGKGuardZones.Where(x => x.ZoneUID != Guid.Empty))
						if (keys.Contains(elementPolygonGKGuardZone.ZoneUID))
							elementPolygonGKGuardZones.Add(elementPolygonGKGuardZone);
					Plan.ElementPolygonGKGuardZones = elementPolygonGKGuardZones;

					keys = GKManager.Devices.Select(item => item.UID).ToList();
					var elementGKDevices = new List<ElementGKDevice>();
					foreach (var elementGKDevice in Plan.ElementGKDevices.Where(x => x.DeviceUID != Guid.Empty))
						if (keys.Contains(elementGKDevice.DeviceUID))
							elementGKDevices.Add(elementGKDevice);
					Plan.ElementGKDevices = elementGKDevices;

					keys = GKManager.Doors.Select(item => item.UID).ToList();
					var elementGKDoors = new List<ElementGKDoor>();
					foreach (var elementDoor in Plan.ElementGKDoors.Where(x => x.DoorUID != Guid.Empty))
						if (keys.Contains(elementDoor.DoorUID))
							elementGKDoors.Add(elementDoor);
					Plan.ElementGKDoors = elementGKDoors;

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

					keys = GKManager.Devices.Select(item => item.UID).ToList();
					var cameraKeys = SystemConfiguration.AllCameras.Select(item => item.UID).ToList();
					var procedureKeys = SystemConfiguration.AutomationConfiguration.Procedures.Select(item => item.Uid).ToList();
					var elementExtensions = new List<ElementBase>();
					foreach (var elementExtension in Plan.ElementExtensions)
					{
						var elementTank = elementExtension as ElementRectangleTank;
						if (elementTank != null)
						{
							if (keys.Contains(elementTank.DeviceUID))
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
							else
							{
								var elementProcedure = elementExtension as ElementProcedure;
								if (elementProcedure != null)
								{
									if (procedureKeys.Contains(elementProcedure.ProcedureUID))
										elementExtensions.Add(elementProcedure);
								}
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