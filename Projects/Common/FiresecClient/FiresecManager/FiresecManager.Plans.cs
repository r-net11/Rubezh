using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using StrazhAPI.Models;
using StrazhAPI.SKD;
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

					var cameraKeys = SystemConfiguration.Cameras.Select(item => item.UID).ToList();
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
			return true;
		}
	}
}