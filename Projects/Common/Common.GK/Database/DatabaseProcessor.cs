using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using XFiresecAPI;
using Common.GK;

namespace Common.GK
{
	public static class DatabaseProcessor
	{
		public static DatabaseManager DatabaseManager;

		public static void Convert()
		{
			XManager.InitializeMissingDefaultProperties();
			InitializeZones();
			InitializeInputOutputdependences();
			InitializeDeviceLogicDependences();
			DatabaseManager = new DatabaseManager();
		}

		static void InitializeZones()
		{
			foreach (var zone in XManager.DeviceConfiguration.Zones)
			{
				zone.Devices = new List<XDevice>();
				zone.KauDatabaseParent = null;
				zone.GkDatabaseParent = null;

				var kauParents = new HashSet<XDevice>();
				foreach (var deviceUID in zone.DeviceUIDs)
				{
					var device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
					if (device == null)
						continue;
					zone.Devices.Add(device);

					var kauParent = device.AllParents.FirstOrDefault(x => x.Driver.DriverType == XDriverType.KAU);
					kauParents.Add(kauParent);
				}

				if (kauParents.Count == 1)
				{
					var kauDevice = kauParents.ToList()[0];
					zone.KauDatabaseParent = kauDevice;
				}
				if (kauParents.Count > 1)
				{
					var kauDevice = kauParents.ToList()[0];
					zone.GkDatabaseParent = kauDevice.Parent;
				}
			}
		}

		static void InitializeInputOutputdependences()
		{
			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				device.ClearBinaryData();
			}
			foreach (var zone in XManager.DeviceConfiguration.Zones)
			{
				zone.ClearBinaryData();
			}

			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				foreach (var stateLogic in device.DeviceLogic.StateLogics)
				{
					foreach (var clause in stateLogic.Clauses)
					{
						foreach (var deviceUID in clause.Devices)
						{
							var clauseDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
							device.InputDevices.Add(clauseDevice);
							clauseDevice.OutputDevices.Add(device);
						}
						foreach (var zoneNo in clause.Zones)
						{
							var zone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == zoneNo);
							device.InputZones.Add(zone);
							zone.OutputDevices.Add(device);
						}
					}
				}
			}

			foreach (var zone in XManager.DeviceConfiguration.Zones)
			{
				foreach (var device in zone.Devices)
				{
					zone.InputDevices.Add(device);
					device.OutputZones.Add(zone);
				}
			}
		}

		static void InitializeDeviceLogicDependences()
		{
			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				device.DeviceLogic.DependentDevices = new List<XDevice>();
				device.DeviceLogic.DependentZones = new List<XZone>();

				foreach (var stateLogic in device.DeviceLogic.StateLogics)
				{
					foreach (var clause in stateLogic.Clauses)
					{
						foreach (var deviceUID in clause.Devices)
						{
							var dependentDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
							device.DeviceLogic.DependentDevices.Add(dependentDevice);
						}
						foreach (var zoneNo in clause.Zones)
						{
							var dependentZone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == zoneNo);
							device.DeviceLogic.DependentZones.Add(dependentZone);
						}
					}
				}
			}
		}
	}
}