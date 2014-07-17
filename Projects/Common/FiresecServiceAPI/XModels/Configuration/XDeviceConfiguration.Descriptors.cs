using System.Collections.Generic;
using System.Linq;
using FiresecClient;

namespace FiresecAPI.GK
{
	public partial class XDeviceConfiguration
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

				var gkParents = new HashSet<XDevice>();
				foreach (var device in zone.Devices)
				{
					var gkParent = device.AllParents.FirstOrDefault(x => x.DriverType == XDriverType.GK);
					gkParents.Add(gkParent);
				}

				var gkDevice = gkParents.FirstOrDefault();
				if (gkDevice != null)
				{
					zone.GkDatabaseParent = gkDevice;
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
				LinkDeviceLogic(device, device.DeviceLogic.ClausesGroup.Clauses);
				LinkDeviceLogic(device, device.DeviceLogic.OffClausesGroup.Clauses);
			}

			foreach (var zone in Zones)
			{
				foreach (var device in zone.Devices)
				{
					LinkXBases(zone, device);
				}
				LinkXBases(zone, zone);
			}

			foreach (var direction in Directions)
			{
				foreach (var zone in direction.InputZones)
				{
					LinkXBases(direction, zone);
				}

				foreach (var device in direction.InputDevices)
				{
					LinkXBases(direction, device);
				}
			}

			foreach (var pumpStation in PumpStations)
			{
				LinkDeviceLogic(pumpStation, pumpStation.StartLogic.ClausesGroup.Clauses);
				LinkDeviceLogic(pumpStation, pumpStation.StopLogic.ClausesGroup.Clauses);
				LinkDeviceLogic(pumpStation, pumpStation.AutomaticOffLogic.ClausesGroup.Clauses);
			}

			foreach (var mpt in MPTs)
			{
				LinkDeviceLogic(mpt, mpt.StartLogic.ClausesGroup.Clauses);
			}

			foreach (var delay in Delays)
			{
				LinkDeviceLogic(delay, delay.DeviceLogic.ClausesGroup.Clauses);
			}

			foreach (var guardZone in GuardZones)
			{
				foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
				{
					LinkXBases(guardZone, guardZoneDevice.Device);
					if (guardZoneDevice.Device.DriverType == XDriverType.RSR2_GuardDetector)
					{
						LinkXBases(guardZoneDevice.Device, guardZone);
					}
				}
				LinkXBases(guardZone, guardZone);
			}
		}

		void LinkDeviceLogic(XBase xBase, List<XClause> clauses)
		{
			if (clauses != null)
			{
				foreach (var clause in clauses)
				{
					foreach (var clauseDevice in clause.Devices)
						LinkXBases(xBase, clauseDevice);
					foreach (var zone in clause.Zones)
						LinkXBases(xBase, zone);
					foreach (var direction in clause.Directions)
						LinkXBases(xBase, direction);
					foreach (var mpt in clause.MPTs)
						LinkXBases(xBase, mpt);
					foreach (var delay in clause.Delays)
						LinkXBases(xBase, delay);
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
					direction.GkDatabaseParent = inputDevice.AllParents.FirstOrDefault(x => x.DriverType == XDriverType.GK);
				}

				var outputDevice = direction.OutputDevices.FirstOrDefault();
				if (outputDevice != null)
				{
					direction.GkDatabaseParent = outputDevice.AllParents.FirstOrDefault(x => x.DriverType == XDriverType.GK);
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
					pumpStation.GkDatabaseParent = inputDevice.AllParents.FirstOrDefault(x => x.DriverType == XDriverType.GK);
				}

				var outputDevice = pumpStation.NSDevices.FirstOrDefault();
				if (outputDevice != null)
				{
					pumpStation.GkDatabaseParent = outputDevice.AllParents.FirstOrDefault(x => x.DriverType == XDriverType.GK);
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
					mpt.GkDatabaseParent = inputDevice.AllParents.FirstOrDefault(x => x.DriverType == XDriverType.GK);
				}

				var outputDevice = mpt.Devices.FirstOrDefault();
				if (outputDevice != null)
				{
					mpt.GkDatabaseParent = outputDevice.AllParents.FirstOrDefault(x => x.DriverType == XDriverType.GK);
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
					delay.GkDatabaseParent = inputDevice.AllParents.FirstOrDefault(x => x.DriverType == XDriverType.GK);
				}
			}
		}

		void PrepareGuardZones()
		{
			foreach (var guardZone in GuardZones)
			{
				guardZone.KauDatabaseParent = null;
				guardZone.GkDatabaseParent = null;

				var gkParents = new HashSet<XDevice>();
				foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
				{
					var gkParent = guardZoneDevice.Device.AllParents.FirstOrDefault(x => x.DriverType == XDriverType.GK);
					gkParents.Add(gkParent);
				}

				var gkDevice = gkParents.FirstOrDefault();
				if (gkDevice != null)
				{
					guardZone.GkDatabaseParent = gkDevice;
				}
			}
		}

		void PrepareCodes()
		{
			foreach (var code in Codes)
			{
				code.KauDatabaseParent = null;
				code.GkDatabaseParent = XManager.Devices.FirstOrDefault(x=>x.DriverType == XDriverType.GK);
			}
		}

		public static void LinkXBases(XBase value, XBase dependsOn)
		{
			AddInputOutputObject(value.InputXBases, dependsOn);
			AddInputOutputObject(dependsOn.OutputXBases, value);
		}

		static void AddInputOutputObject(List<XBase> objects, XBase newObject)
		{
			if (!objects.Contains(newObject))
				objects.Add(newObject);
		}
	}
}