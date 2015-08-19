using FiresecAPI.GK;
using FiresecClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace GKProcessor.DescriptorsDatabase
{
	public class TestDescriptorsCreator
	{
		public void SetDependentDescriptors()
		{
			var gkBases = new List<GKBase>();
			foreach (var device in GKManager.Devices)
			{
				device.DescriptorDependentObjects = device.Logic.GetObjects();
				gkBases.Add(device);
			}

			foreach (var zone in GKManager.Zones)
			{
				zone.DescriptorDependentObjects = new List<GKBase>(zone.Devices);
				gkBases.Add(zone);
			}

			foreach (var direction in GKManager.Directions)
			{
				direction.DescriptorDependentObjects = direction.Logic.GetObjects();
				gkBases.Add(direction);
			}

			foreach (var delay in GKManager.Delays)
			{
				delay.DescriptorDependentObjects = delay.Logic.GetObjects();
				gkBases.Add(delay);
			}

			foreach (var code in GKManager.DeviceConfiguration.Codes)
			{
				gkBases.Add(code);
			}

			foreach (var guardZone in GKManager.GuardZones)
			{
				guardZone.DescriptorDependentObjects = new List<GKBase>();
				foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
				{
					guardZone.DescriptorDependentObjects.Add(guardZoneDevice.Device);
					if (guardZoneDevice.Device.DriverType == GKDriverType.RSR2_CodeReader || guardZoneDevice.Device.DriverType == GKDriverType.RSR2_CardReader)
					{
						guardZoneDevice.Device.DescriptorDependentObjects.Add(guardZone);

						var codeUIDs = new List<Guid>();
						codeUIDs.AddRange(guardZoneDevice.CodeReaderSettings.SetGuardSettings.CodeUIDs);
						codeUIDs.AddRange(guardZoneDevice.CodeReaderSettings.ResetGuardSettings.CodeUIDs);
						codeUIDs.AddRange(guardZoneDevice.CodeReaderSettings.ChangeGuardSettings.CodeUIDs);
						codeUIDs.AddRange(guardZoneDevice.CodeReaderSettings.AlarmSettings.CodeUIDs);

						foreach (var codeUID in codeUIDs)
						{
							var code = GKManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == codeUID);
							if (code != null)
							{
								guardZone.DescriptorDependentObjects.Add(code);
								code.DescriptorDependentObjects.Add(guardZone);
							}
						}
					}
				}
				gkBases.Add(guardZone);
			}

			foreach (var pumpStation in GKManager.PumpStations)
			{
				pumpStation.DescriptorDependentObjects = new List<GKBase>();
				pumpStation.DescriptorDependentObjects.AddRange(pumpStation.StartLogic.GetObjects());
				pumpStation.DescriptorDependentObjects.AddRange(pumpStation.StopLogic.GetObjects());
				pumpStation.DescriptorDependentObjects.AddRange(pumpStation.AutomaticOffLogic.GetObjects());
				foreach (var nsDevice in pumpStation.NSDevices)
				{
					pumpStation.DescriptorDependentObjects.Add(nsDevice);
					nsDevice.DescriptorDependentObjects.Add(pumpStation);
				}

				gkBases.Add(pumpStation);
			}

			foreach (var mpt in GKManager.MPTs)
			{
				mpt.DescriptorDependentObjects = new List<GKBase>();
				mpt.DescriptorDependentObjects.AddRange(mpt.MptLogic.GetObjects());
				foreach (var mptDevice in mpt.MPTDevices)
				{
					mpt.DescriptorDependentObjects.Add(mptDevice.Device);
					mptDevice.Device.DescriptorDependentObjects.Add(mpt);
					var codeUIDs = new List<Guid>();
					codeUIDs.AddRange(mptDevice.CodeReaderSettings.AutomaticOnSettings.CodeUIDs);
					codeUIDs.AddRange(mptDevice.CodeReaderSettings.AutomaticOffSettings.CodeUIDs);
					codeUIDs.AddRange(mptDevice.CodeReaderSettings.StartSettings.CodeUIDs);
					codeUIDs.AddRange(mptDevice.CodeReaderSettings.StopSettings.CodeUIDs);

					foreach (var codeUID in codeUIDs)
					{
						var code = GKManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == codeUID);
						if (code != null)
						{
							mpt.DescriptorDependentObjects.Add(code);
							code.DescriptorDependentObjects.Add(mpt);
						}
					}
				}

				gkBases.Add(mpt);
			}

			foreach (var door in GKManager.Doors)
			{
				door.DescriptorDependentObjects = new List<GKBase>();
				door.DescriptorDependentObjects.AddRange(door.OpenRegimeLogic.GetObjects());
				door.DescriptorDependentObjects.AddRange(door.NormRegimeLogic.GetObjects());
				door.DescriptorDependentObjects.AddRange(door.CloseRegimeLogic.GetObjects());

				if (door.EnterDevice != null)
				{
					door.DescriptorDependentObjects.Add(door.EnterDevice);
					door.EnterDevice.DescriptorDependentObjects.Add(door);
				}

				if (door.ExitDevice != null)
				{
					door.DescriptorDependentObjects.Add(door.ExitDevice);
					door.ExitDevice.DescriptorDependentObjects.Add(door);
				}

				if (door.EnterButton != null)
				{
					door.DescriptorDependentObjects.Add(door.EnterButton);
					door.EnterButton.DescriptorDependentObjects.Add(door);
				}

				if (door.ExitButton != null)
				{
					door.DescriptorDependentObjects.Add(door.ExitButton);
					door.ExitButton.DescriptorDependentObjects.Add(door);
				}

				if (door.LockDevice != null)
				{
					door.DescriptorDependentObjects.Add(door.LockDevice);
					door.LockDevice.DescriptorDependentObjects.Add(door);
				}

				if (door.LockDeviceExit != null)
				{
					door.DescriptorDependentObjects.Add(door.LockDeviceExit);
					door.LockDeviceExit.DescriptorDependentObjects.Add(door);
				}

				if (door.LockControlDevice != null)
				{
					door.DescriptorDependentObjects.Add(door.LockControlDevice);
					door.LockControlDevice.DescriptorDependentObjects.Add(door);
				}

				if (door.LockControlDeviceExit != null)
				{
					door.DescriptorDependentObjects.Add(door.LockControlDeviceExit);
					door.LockControlDeviceExit.DescriptorDependentObjects.Add(door);
				}

				gkBases.Add(door);
			}

			gkBases.ForEach(x => x.IsReady = false);
			while (!gkBases.All(x => x.IsReady))
			{
				foreach (var gkBase in gkBases)
				{
					gkBase.CalculateNext();
				}
			}

			foreach (var gkBase in gkBases)
			{
				var kauParents = new HashSet<GKDevice>();
				var gkParents = new HashSet<GKDevice>();
				foreach (var dependentObject in gkBase.DescriptorDependentObjects)
				{
					if (dependentObject is GKDevice)
					{
						var device = dependentObject as GKDevice;
						if (device.KAUParent != null)
						{
							kauParents.Add(device.KAUParent);
						}
						if (device.GKParent != null)
						{
							gkParents.Add(device.GKParent);
						}
					}
				}
				foreach (var dependentObject in gkBase.DescriptorDependentObjects)
				{
					if (dependentObject is GKDoor)
					{
						kauParents.Clear();
					}
				}

				foreach (var dependentObject in gkBase.DescriptorDependentObjects)
				{
					if (dependentObject is GKGuardZone)
					{
						var guardZone = dependentObject as GKGuardZone;

						foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
						{
							if (guardZoneDevice.Device.DriverType == GKDriverType.RSR2_CodeReader || guardZoneDevice.Device.DriverType == GKDriverType.RSR2_CardReader)
							{
								var hasAccessLevel = guardZoneDevice.CodeReaderSettings.SetGuardSettings.AccessLevel > 0 ||
								guardZoneDevice.CodeReaderSettings.ResetGuardSettings.AccessLevel > 0 ||
								guardZoneDevice.CodeReaderSettings.ChangeGuardSettings.AccessLevel > 0 ||
								guardZoneDevice.CodeReaderSettings.AlarmSettings.AccessLevel > 0;
								if (hasAccessLevel)
								{
									kauParents.Clear();
								}
							}
						}
					}
				}

				gkBase.KauDatabaseParent = null;
				gkBase.GkDatabaseParent = null;

				if (kauParents.Count == 1)
				{
					gkBase.KauDatabaseParent = kauParents.FirstOrDefault();
					gkBase.IsLogicOnKau = true;
				}
				else
				{
					gkBase.GkDatabaseParent = gkParents.FirstOrDefault();
					gkBase.IsLogicOnKau = false;
				}
			}
		}
	}
}