using Common;
using RubezhAPI;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RubezhClient
{
	public partial class ClientManager
	{
		public static void InvalidatePlans()
		{
			try
			{
				foreach (var Plan in PlansConfiguration.AllPlans)
				{
					var keys = PlansConfiguration.AllPlans.Select(item => item.UID).ToList();
					var elementSubPlans = new List<ElementRectangleSubPlan>();
					foreach (var elementSubPlan in Plan.ElementSubPlans)
						if (keys.Contains(elementSubPlan.PlanUID))
							elementSubPlans.Add(elementSubPlan);
					Plan.ElementSubPlans = elementSubPlans;

					var elementPolygonSubPlans = new List<ElementPolygonSubPlan>();
					foreach (var elementSubPlan in Plan.ElementPolygonSubPlans)
						if (keys.Contains(elementSubPlan.PlanUID))
							elementPolygonSubPlans.Add(elementSubPlan);
					Plan.ElementPolygonSubPlans = elementPolygonSubPlans;

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

					keys = GKManager.SKDZones.Select(item => item.UID).ToList();
					var elementRectangleGKSKDZones = new List<ElementRectangleGKSKDZone>();
					foreach (var elementRectangleGKSKDZone in Plan.ElementRectangleGKSKDZones.Where(x => x.ZoneUID != Guid.Empty))
						if (keys.Contains(elementRectangleGKSKDZone.ZoneUID))
							elementRectangleGKSKDZones.Add(elementRectangleGKSKDZone);
					Plan.ElementRectangleGKSKDZones = elementRectangleGKSKDZones;

					var elementPolygonGKSKDZones = new List<ElementPolygonGKSKDZone>();
					foreach (var elementPolygonGKSKDZone in Plan.ElementPolygonGKSKDZones.Where(x => x.ZoneUID != Guid.Empty))
						if (keys.Contains(elementPolygonGKSKDZone.ZoneUID))
							elementPolygonGKSKDZones.Add(elementPolygonGKSKDZone);
					Plan.ElementPolygonGKSKDZones = elementPolygonGKSKDZones;

					keys = GKManager.Delays.Select(item => item.UID).ToList();
					var elementRectangleGKDelays = Plan.ElementRectangleGKDelays
						.Where(x => x.DelayUID != Guid.Empty && keys.Contains(x.DelayUID))
						.ToList();
					Plan.ElementRectangleGKDelays = elementRectangleGKDelays;

					var elementPolygonGKDelays = Plan.ElementPolygonGKDelays
						.Where(x => x.DelayUID != Guid.Empty && keys.Contains(x.DelayUID))
						.ToList();
					Plan.ElementPolygonGKDelays = elementPolygonGKDelays;

					keys = GKManager.PumpStations.Select(item => item.UID).ToList();
					Plan.ElementRectangleGKPumpStations = Plan.ElementRectangleGKPumpStations
						.Where(x => x.ItemUID != Guid.Empty && keys.Contains(x.ItemUID))
						.ToList();

					Plan.ElementPolygonGKPumpStations = Plan.ElementPolygonGKPumpStations
						.Where(x => x.ItemUID != Guid.Empty && keys.Contains(x.ItemUID))
						.ToList();

					keys = GKManager.Doors.Select(item => item.UID).ToList();
					var elementGKDoors = new List<ElementGKDoor>();
					foreach (var elementDoor in Plan.ElementGKDoors.Where(x => x.DoorUID != Guid.Empty))
						if (keys.Contains(elementDoor.DoorUID))
							elementGKDoors.Add(elementDoor);
					Plan.ElementGKDoors = elementGKDoors;

					keys = GKManager.Devices.Select(item => item.UID).ToList();
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
				Logger.Error(e, "ClientManager.InvalidatePlans");
			}
		}
		public static bool PlanValidator(PlansConfiguration configuration)
		{
			return true;
		}
	}
}