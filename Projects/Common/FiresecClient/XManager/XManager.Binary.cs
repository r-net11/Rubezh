using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace FiresecClient
{
	public partial class XManager
	{
		public static void Prepare()
		{
			PrepareZones();
			PrepareInputOutputdependences();
			PrepareDeviceLogicDependences();
			PrepareDirections();
		}

		static void PrepareZones()
		{
			foreach (var zone in DeviceConfiguration.Zones)
			{
				zone.KauDatabaseParent = null;
				zone.GkDatabaseParent = null;

				var kauParents = new HashSet<XDevice>();
				foreach (var device in zone.Devices)
				{
					var kauParent = device.AllParents.FirstOrDefault(x => x.Driver.DriverType == XDriverType.KAU);
					kauParents.Add(kauParent);
				}

				if (kauParents.Count == 1)
				{
					var kauDevice = kauParents.First();
					zone.KauDatabaseParent = kauDevice;
				}
				if (kauParents.Count > 1)
				{
					var kauDevice = kauParents.First();
					zone.GkDatabaseParent = kauDevice.Parent;
				}
			}
		}

		static void PrepareInputOutputdependences()
		{
			foreach (var device in DeviceConfiguration.Devices)
			{
				device.ClearBinaryData();
			}
			foreach (var zone in DeviceConfiguration.Zones)
			{
				zone.ClearBinaryData();
			}
			foreach (var direction in DeviceConfiguration.Directions)
			{
				direction.ClearBinaryData();
			}

			foreach (var device in DeviceConfiguration.Devices)
			{
				foreach (var stateLogic in device.DeviceLogic.StateLogics)
				{
					foreach (var clause in stateLogic.Clauses)
					{
						foreach (var clauseDevice in clause.XDevices)
						{
							device.InputObjects.Add(clauseDevice);
							clauseDevice.OutputObjects.Add(device);
						}
						foreach (var zone in clause.XZones)
						{
							device.InputObjects.Add(zone);
							zone.OutputObjects.Add(device);
						}
					}
				}
			}

			foreach (var zone in DeviceConfiguration.Zones)
			{
				foreach (var device in zone.Devices)
				{
					zone.InputObjects.Add(device);
					device.OutputObjects.Add(zone);
				}
			}

			foreach (var direction in DeviceConfiguration.Directions)
			{
				foreach (var zone in direction.XZones)
				{
					direction.InputObjects.Add(zone);
					zone.OutputObjects.Add(direction);
				}

				foreach (var directionDevice in direction.DirectionDevices)
				{
					direction.OutputObjects.Add(directionDevice.Device);
					directionDevice.Device.InputObjects.Add(direction);
				}
			}
		}

		static void PrepareDeviceLogicDependences()
		{
			foreach (var device in DeviceConfiguration.Devices)
			{
				device.DeviceLogic.DependentDevices = new List<XDevice>();
				device.DeviceLogic.DependentZones = new List<XZone>();

				foreach (var stateLogic in device.DeviceLogic.StateLogics)
				{
					foreach (var clause in stateLogic.Clauses)
					{
						foreach (var deviceUID in clause.Devices)
						{
							var dependentDevice = DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
							device.DeviceLogic.DependentDevices.Add(dependentDevice);
						}
						foreach (var zoneNo in clause.Zones)
						{
							var dependentZone = DeviceConfiguration.Zones.FirstOrDefault(x => x.No == zoneNo);
							device.DeviceLogic.DependentZones.Add(dependentZone);
						}
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
				var zone = direction.XZones.FirstOrDefault();
				if (zone != null)
				{
					if (zone.KauDatabaseParent != null)
						direction.GkDatabaseParent = zone.KauDatabaseParent.Parent;
					if (zone.GkDatabaseParent != null)
						direction.GkDatabaseParent = zone.GkDatabaseParent;
				}
			}
		}
	}
}