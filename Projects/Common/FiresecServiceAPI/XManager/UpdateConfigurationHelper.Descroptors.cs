using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using Common;

namespace FiresecClient
{
	public static partial class UpdateConfigurationHelper
	{
		public static void PrepareDescriptors(XDeviceConfiguration deviceConfiguration)
		{
			DeviceConfiguration = deviceConfiguration;
			PrepareZones();
			PrepareInputOutputDependences();
			PrepareDeviceLogicDependences();
			PrepareDirections();
			PreparePumpStations();
		}

		static void PrepareZones()
		{
			foreach (var zone in DeviceConfiguration.Zones)
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

		static void PrepareInputOutputDependences()
		{
			foreach (var device in DeviceConfiguration.Devices)
			{
				device.ClearDescriptor();
			}
			foreach (var zone in DeviceConfiguration.Zones)
			{
				zone.ClearDescriptor();
			}
			foreach (var direction in DeviceConfiguration.Directions)
			{
				direction.ClearDescriptor();
			}
			foreach (var pumpStation in DeviceConfiguration.PumpStations)
			{
				pumpStation.ClearDescriptor();
			}

			foreach (var device in DeviceConfiguration.Devices)
			{
				LinkDeviceLogic(device, device.DeviceLogic);
			}

			foreach (var zone in DeviceConfiguration.Zones)
			{
				LinkXBases(zone, zone);
				foreach (var device in zone.Devices)
				{
					LinkXBases(zone, device);
				}
			}

			foreach (var direction in DeviceConfiguration.Directions)
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

			foreach (var pumpStation in DeviceConfiguration.PumpStations)
			{
				LinkDeviceLogic(pumpStation, pumpStation.StartLogic);
				LinkDeviceLogic(pumpStation, pumpStation.ForbidLogic);
				LinkDeviceLogic(pumpStation, pumpStation.AutomaticOffLogic);
			}
		}

		static void LinkDeviceLogic(XBase xBase, XDeviceLogic deviceLogic)
		{
			foreach (var clause in deviceLogic.Clauses)
			{
				foreach (var zone in clause.Zones)
				{
					LinkXBases(xBase, zone);
				}
				foreach (var clauseDevice in clause.Devices)
				{
					LinkXBases(xBase, clauseDevice);
				}
				foreach (var direction in clause.Directions)
				{
					LinkXBases(xBase, direction);
				}
			}
		}

		static void PrepareDeviceLogicDependences()
		{
			foreach (var device in DeviceConfiguration.Devices)
			{
				device.DeviceLogic.DependentZones = new List<XZone>();
				device.DeviceLogic.DependentDevices = new List<XDevice>();
				device.DeviceLogic.DependentDirections = new List<XDirection>();

				foreach (var clause in device.DeviceLogic.Clauses)
				{
					foreach (var clauseZone in clause.Zones)
					{
						device.DeviceLogic.DependentZones.Add(clauseZone);
					}
					foreach (var clauseDevice in clause.Devices)
					{
						device.DeviceLogic.DependentDevices.Add(clauseDevice);
					}
					foreach (var direction in clause.Directions)
					{
						device.DeviceLogic.DependentDirections.Add(direction);
					}
				}
			}
		}

		static void PrepareDirections()
		{
			foreach (var direction in DeviceConfiguration.Directions)
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

		static void PreparePumpStations()
		{
			foreach (var pumpStation in DeviceConfiguration.PumpStations)
			{
				pumpStation.KauDatabaseParent = null;
				pumpStation.GkDatabaseParent = null;

				var inputZone = pumpStation.InputZones.FirstOrDefault();
				if (inputZone != null)
				{
					if (inputZone.GkDatabaseParent != null)
						pumpStation.GkDatabaseParent = inputZone.GkDatabaseParent;
				}

				var inputDevice = pumpStation.InputDevices.FirstOrDefault();
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

		public static void LinkXBases(XBase inputXBase, XBase outputXBase)
		{
			AddInputOutputObject(inputXBase.InputXBases, outputXBase);
			AddInputOutputObject(outputXBase.OutputXBases, inputXBase);
		}

		static void AddInputOutputObject(List<XBase> objects, XBase newObject)
		{
			if (!objects.Contains(newObject))
				objects.Add(newObject);
		}
	}
}