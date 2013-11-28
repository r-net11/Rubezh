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
				LinkXBasees(zone, zone);
			}
			foreach (var direction in DeviceConfiguration.Directions)
			{
				direction.ClearDescriptor();
			}

			foreach (var device in DeviceConfiguration.Devices)
			{
				foreach (var clause in device.DeviceLogic.Clauses)
				{
					foreach (var zone in clause.Zones)
					{
						LinkXBasees(device, zone);
					}
					foreach (var clauseDevice in clause.Devices)
					{
						LinkXBasees(device, clauseDevice);
					}
					foreach (var direction in clause.Directions)
					{
						LinkXBasees(device, direction);
					}
				}
			}

			foreach (var zone in DeviceConfiguration.Zones)
			{
				foreach (var device in zone.Devices)
				{
					LinkXBasees(zone, device);
				}
			}

			foreach (var direction in DeviceConfiguration.Directions)
			{
				foreach (var zone in direction.InputZones)
				{
					LinkXBasees(direction, zone);
				}

				foreach (var device in direction.InputDevices)
				{
					LinkXBasees(direction, device);
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

		public static void LinkXBasees(XBase inputXBase, XBase outputXBase)
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