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
			foreach (var zone in Zones)
			{
				zone.KauDatabaseParent = null;
				zone.GkDatabaseParent = null;

				var kauParents = new HashSet<XDevice>();
				foreach (var device in zone.Devices)
				{
					var kauParent = device.AllParents.FirstOrDefault(x => x.Driver.IsKauOrRSR2Kau);
					kauParents.Add(kauParent);
				}

				var kauDevice = kauParents.FirstOrDefault();
				if (kauDevice != null)
				{
					zone.GkDatabaseParent = kauDevice.Parent;
				}
			}
		}

		static void PrepareInputOutputdependences()
		{
			foreach (var device in Devices)
			{
				device.ClearBinaryData();
			}
			foreach (var zone in Zones)
			{
				zone.ClearBinaryData();
				LinkBinaryObjects(zone, zone);
			}
			foreach (var direction in Directions)
			{
				direction.ClearBinaryData();
			}

            foreach (var device in Devices)
            {
                foreach (var clause in device.DeviceLogic.Clauses)
                {
					foreach (var zone in clause.Zones)
					{
						LinkBinaryObjects(device, zone);
					}
                    foreach (var clauseDevice in clause.Devices)
                    {
						LinkBinaryObjects(device, clauseDevice);
                    }
					foreach (var direction in clause.Directions)
                    {
						LinkBinaryObjects(device, direction);
                    }
                }
            }

			foreach (var zone in Zones)
			{
				foreach (var device in zone.Devices)
				{
					LinkBinaryObjects(zone, device);
				}
			}

			foreach (var direction in Directions)
			{
				foreach (var zone in direction.InputZones)
				{
					LinkBinaryObjects(direction, zone);
				}

				foreach (var device in direction.InputDevices)
				{
					LinkBinaryObjects(direction, device);
				}
			}
		}

        static void PrepareDeviceLogicDependences()
        {
            foreach (var device in Devices)
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
			foreach (var direction in Directions)
			{
				direction.KauDatabaseParent = null;
				direction.GkDatabaseParent = null;

				var inputZone = direction.InputZones.FirstOrDefault();
				if (inputZone != null)
				{
					if (inputZone.KauDatabaseParent != null)
						direction.GkDatabaseParent = inputZone.KauDatabaseParent.Parent;
					if (inputZone.GkDatabaseParent != null)
						direction.GkDatabaseParent = inputZone.GkDatabaseParent;
				}

				var inputDevice = direction.InputDevices.FirstOrDefault();
				if (inputDevice != null)
				{
					direction.GkDatabaseParent = inputDevice.AllParents.FirstOrDefault(x => x.Driver.DriverType == XDriverType.GK);
				}

				var outputDevice = direction.OutputDevices.FirstOrDefault();
				if (outputDevice != null)
				{
					direction.GkDatabaseParent = outputDevice.AllParents.FirstOrDefault(x => x.Driver.DriverType == XDriverType.GK);
				}
			}
		}

		public static void LinkBinaryObjects(XBase inputXBase, XBase outputXBase)
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