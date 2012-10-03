using System.Collections.Generic;
using System.Linq;
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

				if (kauParents.Count > 0)
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
                zone.InputObjects.Add(zone);
				zone.OutputObjects.Add(zone);
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
				foreach (var zone in direction.Zones)
				{
					direction.InputObjects.Add(zone);
					zone.OutputObjects.Add(direction);
				}

				foreach (var device in direction.Devices)
				{
					direction.OutputObjects.Add(device);
					device.InputObjects.Add(direction);
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
						foreach (var zoneUID in clause.Zones)
						{
							var dependentZone = DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
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
				var zone = direction.Zones.FirstOrDefault();
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