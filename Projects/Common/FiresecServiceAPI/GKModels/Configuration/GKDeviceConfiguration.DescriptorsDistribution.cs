using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FiresecClient;
using System;

namespace FiresecAPI.GK
{
	public partial class GKDeviceConfiguration
	{
		public void PrepareDescriptors()
		{
			Codes.ForEach(x => x.ChildDescriptors = new List<GKBase>());
			var gkBases = new List<GKBase>();
			foreach (var device in Devices)
			{
				device.ChildDescriptors = device.Logic.GetObjects();
				gkBases.Add(device);
			}

			foreach (var zone in Zones)
			{
				zone.ChildDescriptors = new List<GKBase>(zone.Devices);
				gkBases.Add(zone);
			}

			foreach (var direction in Directions)
			{
				direction.ChildDescriptors = direction.Logic.GetObjects();
				gkBases.Add(direction);
			}

			foreach (var delay in Delays)
			{
				delay.ChildDescriptors = delay.Logic.GetObjects();
				gkBases.Add(delay);
			}

			foreach (var code in Codes)
			{
				gkBases.Add(code);
			}

			foreach (var guardZone in GuardZones)
			{
				guardZone.ChildDescriptors = new List<GKBase>();
				foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
				{
					guardZone.ChildDescriptors.Add(guardZoneDevice.Device);
					if (guardZoneDevice.Device.DriverType == GKDriverType.RSR2_GuardDetector || guardZoneDevice.Device.DriverType == GKDriverType.RSR2_GuardDetectorSound
						|| guardZoneDevice.Device.DriverType == GKDriverType.RSR2_CodeReader || guardZoneDevice.Device.DriverType == GKDriverType.RSR2_CardReader)
						guardZoneDevice.Device.ChildDescriptors.Add(guardZone);
					if (guardZoneDevice.Device.DriverType == GKDriverType.RSR2_CodeReader || guardZoneDevice.Device.DriverType == GKDriverType.RSR2_CardReader)
					{
						var codeUIDs = new List<Guid>();
						codeUIDs.AddRange(guardZoneDevice.CodeReaderSettings.SetGuardSettings.CodeUIDs);
						codeUIDs.AddRange(guardZoneDevice.CodeReaderSettings.ResetGuardSettings.CodeUIDs);
						codeUIDs.AddRange(guardZoneDevice.CodeReaderSettings.ChangeGuardSettings.CodeUIDs);
						codeUIDs.AddRange(guardZoneDevice.CodeReaderSettings.AlarmSettings.CodeUIDs);

						foreach (var codeUID in codeUIDs)
						{
							var code = Codes.FirstOrDefault(x => x.UID == codeUID);
							if (code != null)
							{
								guardZone.ChildDescriptors.Add(code);
								code.ChildDescriptors.Add(guardZone);
							}
						}
					}
				}
				gkBases.Add(guardZone);
			}

			foreach (var pumpStation in PumpStations)
			{
				pumpStation.ChildDescriptors = new List<GKBase>();
				pumpStation.ChildDescriptors.AddRange(pumpStation.StartLogic.GetObjects());
				pumpStation.ChildDescriptors.AddRange(pumpStation.StopLogic.GetObjects());
				pumpStation.ChildDescriptors.AddRange(pumpStation.AutomaticOffLogic.GetObjects());
				foreach (var nsDevice in pumpStation.NSDevices)
				{
					pumpStation.ChildDescriptors.Add(nsDevice);
					pumpStation.ChildDescriptors.AddRange(nsDevice.NSLogic.GetObjects());
					nsDevice.ChildDescriptors.Add(pumpStation);
				}

				gkBases.Add(pumpStation);
			}

			foreach (var mpt in MPTs)
			{
				mpt.ChildDescriptors = new List<GKBase>();
				mpt.ChildDescriptors.AddRange(mpt.MptLogic.GetObjects());
				foreach (var mptDevice in mpt.MPTDevices)
				{
					mpt.ChildDescriptors.Add(mptDevice.Device);
					if (mptDevice.MPTDeviceType != GKMPTDeviceType.HandAutomaticOff && mptDevice.MPTDeviceType != GKMPTDeviceType.HandAutomaticOn &&
						mptDevice.MPTDeviceType != GKMPTDeviceType.HandStart && mptDevice.MPTDeviceType != GKMPTDeviceType.HandStop)
					mptDevice.Device.ChildDescriptors.Add(mpt);
					var codeUIDs = new List<Guid>();
					codeUIDs.AddRange(mptDevice.CodeReaderSettings.MPTSettings.CodeUIDs);

					foreach (var codeUID in codeUIDs)
					{
						var code = Codes.FirstOrDefault(x => x.UID == codeUID);
						if (code != null)
						{
							mpt.ChildDescriptors.Add(code);
							code.ChildDescriptors.Add(mpt);
						}
					}
				}

				gkBases.Add(mpt);
			}

			foreach (var door in Doors)
			{
				door.ChildDescriptors = new List<GKBase>();
				door.ChildDescriptors.AddRange(door.OpenRegimeLogic.GetObjects());
				door.ChildDescriptors.AddRange(door.NormRegimeLogic.GetObjects());
				door.ChildDescriptors.AddRange(door.CloseRegimeLogic.GetObjects());

				if (door.EnterDevice != null)
				{
					door.ChildDescriptors.Add(door.EnterDevice);
					//door.EnterDevice.ChildDescriptors.Add(door);
				}

				if (door.ExitDevice != null)
				{
					door.ChildDescriptors.Add(door.ExitDevice);
					//door.ExitDevice.ChildDescriptors.Add(door);
				}

				if (door.EnterButton != null)
				{
					door.ChildDescriptors.Add(door.EnterButton);
					//door.EnterButton.ChildDescriptors.Add(door);
				}

				if (door.ExitButton != null)
				{
					door.ChildDescriptors.Add(door.ExitButton);
					//door.ExitButton.ChildDescriptors.Add(door);
				}

				if (door.LockDevice != null)
				{
					door.ChildDescriptors.Add(door.LockDevice);
					door.LockDevice.ChildDescriptors.Add(door);
				}

				if (door.LockDeviceExit != null)
				{
					door.ChildDescriptors.Add(door.LockDeviceExit);
					door.LockDeviceExit.ChildDescriptors.Add(door);
				}

				if (door.LockControlDevice != null)
				{
					door.ChildDescriptors.Add(door.LockControlDevice);
					//door.LockControlDevice.ChildDescriptors.Add(door);
				}

				if (door.LockControlDeviceExit != null)
				{
					door.ChildDescriptors.Add(door.LockControlDeviceExit);
					//door.LockControlDeviceExit.ChildDescriptors.Add(door);
				}

				gkBases.Add(door);
			}

			gkBases.ForEach(x => x.ClearDescriptor());
			gkBases.ForEach(x => x.PrepareInputOutputDependences());
			gkBases.ForEach(x => x.IsChildDescriptorsReady = false);
			gkBases.ForEach(x => x.CalculateAllChildDescriptors());

			foreach (var gkBase in gkBases)
			{
				gkBase.KauParents = new HashSet<GKDevice>();
				gkBase.GkParents = new HashSet<GKDevice>();
				gkBase.MagnetToGK = false;

				foreach (var dependentObject in gkBase.ChildDescriptors)
				{
					if (dependentObject is GKDevice)
					{
						var device = dependentObject as GKDevice;
						if (device.KAUParent != null)
						{
							gkBase.KauParents.Add(device.KAUParent);
						}
						if (device.GKParent != null)
						{
							gkBase.GkParents.Add(device.GKParent);
						}
					}
				}

				foreach (var dependentObject in gkBase.ChildDescriptors)
				{
					if (dependentObject is GKDevice)
					{
						var device = dependentObject as GKDevice;
						if (device.KAUParent == null && device.GKParent != null)
						{
							gkBase.KauParents.Clear();
							gkBase.MagnetToGK = true;
						}
					}
					if (dependentObject is GKDoor)
					{
						gkBase.KauParents.Clear();
						gkBase.MagnetToGK = true;
					}
					if (dependentObject is GKGuardZone)
					{
						var guardZone = dependentObject as GKGuardZone;
						if (guardZone.HasAccessLevel)
						{
							gkBase.KauParents.Clear();
							gkBase.MagnetToGK = true;
						}
					}
					if (dependentObject is GKMPT)
					{
						var mpt = dependentObject as GKMPT;
						if (mpt.HasAccessLevel)
						{
							gkBase.KauParents.Clear();
							gkBase.MagnetToGK = true;
						}
					}
				}

				if (gkBase is GKDevice)
				{
					var device = gkBase as GKDevice;
					if (device.KAUParent != null)
					{
						gkBase.KauParents.Add(device.KAUParent);
					}
					else
					{
						gkBase.MagnetToGK = true;
					}
					if (device.GKParent != null)
					{
						gkBase.GkParents.Add(device.GKParent);
					}
				}

				gkBase.KauDatabaseParent = null;
				gkBase.GkDatabaseParent = null;

				if (gkBase.KauParents.Count == 1 && !gkBase.MagnetToGK)
				{
					gkBase.KauDatabaseParent = gkBase.KauParents.FirstOrDefault();
					gkBase.GkDatabaseParent = gkBase.KauDatabaseParent.Parent;
					gkBase.IsLogicOnKau = true;
				}
				else
				{
					gkBase.GkDatabaseParent = gkBase.GkParents.FirstOrDefault();
					gkBase.IsLogicOnKau = false;
				}
			}
		}
	}
}