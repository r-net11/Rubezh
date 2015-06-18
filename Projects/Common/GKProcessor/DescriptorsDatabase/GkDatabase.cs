﻿using System.Collections.Generic;
using FiresecAPI.GK;
using FiresecClient;

namespace GKProcessor
{
	public class GkDatabase : CommonDatabase
	{
		List<GKDoor> Doors { get; set; }
		public List<KauDatabase> KauDatabases { get; set; }

		public GkDatabase(GKDevice gkControllerDevice)
		{
			Doors = new List<GKDoor>();
			KauDatabases = new List<KauDatabase>();
			DatabaseType = DatabaseType.Gk;
			RootDevice = gkControllerDevice;

			AddDevice(gkControllerDevice);
			foreach (var device in gkControllerDevice.Children)
			{
				if (device.DriverType == GKDriverType.GKIndicator || device.DriverType == GKDriverType.GKRele)
				{
					AddDevice(device);
				}
			}
			Devices.ForEach(x => x.GkDatabaseParent = RootDevice);
		}

		void AddDevice(GKDevice device)
		{
			if (!Devices.Contains(device))
			{
				Devices.Add(device);
			}
		}

		public override void BuildObjects()
		{
			AddKauObjects();
			foreach (var pumpStation in GKManager.PumpStations)
			{
				if (pumpStation.GkDatabaseParent == RootDevice)
				{
					PumpStations.Add(pumpStation);
				}
			}

			foreach (var mpt in GKManager.DeviceConfiguration.MPTs)
			{
				if (mpt.GkDatabaseParent == RootDevice)
				{
					MPTs.Add(mpt);
				}
			}

			foreach (var door in GKManager.Doors)
			{
				if (door.GkDatabaseParent == RootDevice)
				{
					Doors.Add(door);
				}
			}

			Descriptors = new List<BaseDescriptor>();
			foreach (var device in Devices)
			{
				device.GKDescriptorNo = NextDescriptorNo;
				var deviceDescriptor = new DeviceDescriptor(device, DatabaseType);
				Descriptors.Add(deviceDescriptor);
			}

			foreach (var pumpStation in PumpStations)
			{
				pumpStation.GKDescriptorNo = NextDescriptorNo;
				var pumpStationDescriptor = new PumpStationDescriptor(this, pumpStation, DatabaseType.Gk);
				Descriptors.Add(pumpStationDescriptor);

				var pumpStationCreator = new PumpStationCreator(this, pumpStation, DatabaseType.Gk);
				pumpStationCreator.Create();
			}

			foreach (var mpt in MPTs)
			{
				mpt.GKDescriptorNo = NextDescriptorNo;
				var mptDescriptor = new MPTDescriptor(this, mpt, DatabaseType.Gk);
				Descriptors.Add(mptDescriptor);

				var mptCreator = new MPTCreator(this, mpt, DatabaseType.Gk);
				mptCreator.Create();
			}

			foreach (var door in Doors)
			{
				door.GKDescriptorNo = NextDescriptorNo;
				var doorDescriptor = new DoorDescriptor(door);
				Descriptors.Add(doorDescriptor);
				if (doorDescriptor.DoorPimDescriptorEnter != null)
				{
					AddPim(door.PimEnter);
					Descriptors.Add(doorDescriptor.DoorPimDescriptorEnter);
				}
				if (doorDescriptor.DoorPimDescriptorExit != null)
				{
					AddPim(door.PimExit);
					Descriptors.Add(doorDescriptor.DoorPimDescriptorExit);
				}
			}

			foreach (var descriptor in Descriptors)
			{
				descriptor.Build();
				descriptor.InitializeAllBytes();
			}
		}

		void AddKauObjects()
		{
			foreach (var kauDatabase in KauDatabases)
			{
				foreach (var descriptor in kauDatabase.Descriptors)
				{
					var gkBase = descriptor.GKBase;
					gkBase.GkDatabaseParent = RootDevice;
					if (gkBase is GKDevice)
					{
						GKDevice device = gkBase as GKDevice;
						AddDevice(device);
					}
				}
			}
		}
	}
}