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
                foreach (var clause in device.DeviceLogic.Clauses)
                {
					foreach (var zone in clause.Zones)
					{
						device.InputObjects.Add(zone);
						zone.OutputObjects.Add(device);
					}
                    foreach (var clauseDevice in clause.Devices)
                    {
                        device.InputObjects.Add(clauseDevice);
                        clauseDevice.OutputObjects.Add(device);
                    }
					foreach (var direction in clause.Directions)
                    {
						device.InputObjects.Add(direction);
						direction.OutputObjects.Add(device);
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
					direction.InputObjects.Add(device);
					device.OutputObjects.Add(direction);
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