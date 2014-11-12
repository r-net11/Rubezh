using System.Collections.Generic;
using System.Linq;
using FiresecClient;

namespace FiresecAPI.GK
{
	public partial class GKDeviceConfiguration
	{
		public void PrepareDescriptors()
		{
			PrepareZones();
			PrepareInputOutputDependences();
			PrepareDirections();
			PreparePumpStations();
			PrepareMPTs();
			PrepareDelays();
			PrepareGuardZones();
			PrepareCodes();
		}

		void PrepareZones()
		{
			foreach (var zone in Zones)
			{
				zone.KauDatabaseParent = null;
				zone.GkDatabaseParent = null;

				var gkParents = new HashSet<GKDevice>();
				foreach (var device in zone.Devices)
				{
					var gkParent = device.AllParents.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
					gkParents.Add(gkParent);
				}

				var gkControllerDevice = gkParents.FirstOrDefault();
				if (gkControllerDevice != null)
				{
					zone.GkDatabaseParent = gkControllerDevice;
				}
			}
		}

		void PrepareInputOutputDependences()
		{
			foreach (var device in Devices)
			{
				device.ClearDescriptor();
			}
			foreach (var zone in Zones)
			{
				zone.ClearDescriptor();
			}
			foreach (var direction in Directions)
			{
				direction.ClearDescriptor();
			}
			foreach (var pumpStation in PumpStations)
			{
				pumpStation.ClearDescriptor();
			}
			foreach (var mpt in MPTs)
			{
				mpt.ClearDescriptor();
			}
			foreach (var delay in Delays)
			{
				delay.ClearDescriptor();
			}
			foreach (var guardZone in GuardZones)
			{
				guardZone.ClearDescriptor();
			}
			foreach (var code in Codes)
			{
				code.ClearDescriptor();
			}

			foreach (var device in Devices)
			{
				LinkLogic(device, device.Logic.OnClausesGroup.Clauses);
				LinkLogic(device, device.Logic.OffClausesGroup.Clauses);
				LinkLogic(device, device.Logic.StopClausesGroup.Clauses);
			}

			foreach (var zone in Zones)
			{
				foreach (var device in zone.Devices)
				{
					LinkGKBases(zone, device);
				}
				LinkGKBases(zone, zone);
			}

			foreach (var direction in Directions)
			{
				foreach (var zone in direction.InputZones)
				{
					LinkGKBases(direction, zone);
				}

				foreach (var device in direction.InputDevices)
				{
					LinkGKBases(direction, device);
				}
			}

			foreach (var pumpStation in PumpStations)
			{
				LinkLogic(pumpStation, pumpStation.StartLogic.OnClausesGroup.Clauses);
				LinkLogic(pumpStation, pumpStation.StopLogic.OnClausesGroup.Clauses);
				LinkLogic(pumpStation, pumpStation.AutomaticOffLogic.OnClausesGroup.Clauses);
			}

			foreach (var mpt in MPTs)
			{
				LinkLogic(mpt, mpt.StartLogic.OnClausesGroup.Clauses);
			}

			foreach (var delay in Delays)
			{
				LinkLogic(delay, delay.Logic.OnClausesGroup.Clauses);
				LinkLogic(delay, delay.Logic.OffClausesGroup.Clauses);
			}

			foreach (var guardZone in GuardZones)
			{
				foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
				{
					LinkGKBases(guardZone, guardZoneDevice.Device);
					if (guardZoneDevice.Device.DriverType == GKDriverType.RSR2_GuardDetector || guardZoneDevice.Device.DriverType == GKDriverType.RSR2_CodeReader)
					{
						LinkGKBases(guardZoneDevice.Device, guardZone);
					}
				}
				LinkGKBases(guardZone, guardZone);
			}
		}

		void LinkLogic(GKBase gkBase, List<GKClause> clauses)
		{
			if (clauses != null)
			{
				foreach (var clause in clauses)
				{
					foreach (var clauseDevice in clause.Devices)
						LinkGKBases(gkBase, clauseDevice);
					foreach (var zone in clause.Zones)
						LinkGKBases(gkBase, zone);
					foreach (var guardZone in clause.GuardZones)
						LinkGKBases(gkBase, guardZone);
					foreach (var direction in clause.Directions)
						LinkGKBases(gkBase, direction);
					foreach (var mpt in clause.MPTs)
						LinkGKBases(gkBase, mpt);
					foreach (var delay in clause.Delays)
						LinkGKBases(gkBase, delay);
				}
			}
		}

		void PrepareDirections()
		{
			foreach (var direction in Directions)
			{
				direction.KauDatabaseParent = null;
				direction.GkDatabaseParent = null;

				var inputZone = direction.InputZones.FirstOrDefault();
				if (inputZone != null)
				{
					if (inputZone.GkDatabaseParent != null)
						direction.GkDatabaseParent = inputZone.GkDatabaseParent;
				}

				var inputDevice = direction.InputDevices.FirstOrDefault();
				if (inputDevice != null)
				{
					direction.GkDatabaseParent = inputDevice.AllParents.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
				}

				var outputDevice = direction.OutputDevices.FirstOrDefault();
				if (outputDevice != null)
				{
					direction.GkDatabaseParent = outputDevice.AllParents.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
				}
			}
		}

		void PreparePumpStations()
		{
			foreach (var pumpStation in PumpStations)
			{
				pumpStation.KauDatabaseParent = null;
				pumpStation.GkDatabaseParent = null;

				var inputZone = pumpStation.ClauseInputZones.FirstOrDefault();
				if (inputZone != null)
				{
					if (inputZone.GkDatabaseParent != null)
						pumpStation.GkDatabaseParent = inputZone.GkDatabaseParent;
				}

				var inputDevice = pumpStation.ClauseInputDevices.FirstOrDefault();
				if (inputDevice != null)
				{
					pumpStation.GkDatabaseParent = inputDevice.AllParents.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
				}

				var outputDevice = pumpStation.NSDevices.FirstOrDefault();
				if (outputDevice != null)
				{
					pumpStation.GkDatabaseParent = outputDevice.AllParents.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
				}
			}
		}

		void PrepareMPTs()
		{
			foreach (var mpt in MPTs)
			{
				mpt.KauDatabaseParent = null;
				mpt.GkDatabaseParent = null;

				var inputZone = mpt.ClauseInputZones.FirstOrDefault();
				if (inputZone != null)
				{
					if (inputZone.GkDatabaseParent != null)
						mpt.GkDatabaseParent = inputZone.GkDatabaseParent;
				}

				var inputDevice = mpt.ClauseInputDevices.FirstOrDefault();
				if (inputDevice != null)
				{
					mpt.GkDatabaseParent = inputDevice.AllParents.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
				}

				var outputDevice = mpt.Devices.FirstOrDefault();
				if (outputDevice != null)
				{
					mpt.GkDatabaseParent = outputDevice.AllParents.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
				}
			}
		}

		void PrepareDelays()
		{
			foreach (var delay in Delays)
			{
				delay.KauDatabaseParent = null;
				delay.GkDatabaseParent = null;

				var inputDirection = delay.ClauseInputDirections.FirstOrDefault();
				if (inputDirection != null)
				{
					delay.GkDatabaseParent = inputDirection.GkDatabaseParent;
				}

				var inputZone = delay.ClauseInputZones.FirstOrDefault();
				if (inputZone != null)
				{
					if (inputZone.GkDatabaseParent != null)
						delay.GkDatabaseParent = inputZone.GkDatabaseParent;
				}

				var inputDevice = delay.ClauseInputDevices.FirstOrDefault();
				if (inputDevice != null)
				{
					delay.GkDatabaseParent = inputDevice.AllParents.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
				}
			}
		}

		void PrepareGuardZones()
		{
			foreach (var guardZone in GuardZones)
			{
				guardZone.KauDatabaseParent = null;
				guardZone.GkDatabaseParent = null;

				var gkParents = new HashSet<GKDevice>();
				foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
				{
					var gkParent = guardZoneDevice.Device.AllParents.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
					gkParents.Add(gkParent);
				}

				var gkControllerDevice = gkParents.FirstOrDefault();
				if (gkControllerDevice != null)
				{
					guardZone.GkDatabaseParent = gkControllerDevice;
				}
			}
		}

		void PrepareCodes()
		{
			foreach (var code in Codes)
			{
				code.KauDatabaseParent = null;
				code.GkDatabaseParent = GKManager.Devices.FirstOrDefault(x=>x.DriverType == GKDriverType.GK);
			}
		}

		public static void LinkGKBases(GKBase value, GKBase dependsOn)
		{
			AddInputOutputObject(value.InputGKBases, dependsOn);
			AddInputOutputObject(dependsOn.OutputGKBases, value);
		}

		static void AddInputOutputObject(List<GKBase> objects, GKBase newObject)
		{
			if (!objects.Contains(newObject))
				objects.Add(newObject);
		}
	}
}