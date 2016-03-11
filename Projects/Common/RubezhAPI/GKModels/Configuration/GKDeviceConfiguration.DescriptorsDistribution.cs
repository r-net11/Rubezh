using System.Collections.Generic;
using System.Linq;
using System;

namespace RubezhAPI.GK
{
	public partial class GKDeviceConfiguration
	{
		public void PrepareDescriptors()
		{
			Codes.ForEach(x => x.ChildDescriptors = new List<GKBase>());
			Devices.ForEach(x => x.ChildDescriptors = new List<GKBase>());
			Zones.ForEach(x => x.ChildDescriptors = new List<GKBase>(x.Devices));
			Directions.ForEach(x => x.ChildDescriptors = new List<GKBase>(x.Logic.GetObjects()));
			Delays.ForEach(x => x.ChildDescriptors = new List<GKBase>(x.Logic.GetObjects()));
			GuardZones.ForEach(x => x.ChildDescriptors = new List<GKBase>());
			PumpStations.ForEach(x => x.ChildDescriptors = new List<GKBase>());
			MPTs.ForEach(x => x.ChildDescriptors = new List<GKBase>());
			Doors.ForEach(x => x.ChildDescriptors = new List<GKBase>());

			var gkBases = Zones.Cast<GKBase>().ToList();
			gkBases.AddRange(Directions);
			gkBases.AddRange(Delays);
			gkBases.AddRange(Codes);

			foreach (var guardZone in GuardZones)
			{
				foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
				{
					guardZone.ChildDescriptors.Add(guardZoneDevice.Device);
					if (guardZoneDevice.Device.DriverType == GKDriverType.RSR2_GuardDetector || guardZoneDevice.Device.DriverType == GKDriverType.RSR2_GuardDetectorSound
						|| guardZoneDevice.Device.DriverType == GKDriverType.RSR2_HandGuardDetector || guardZoneDevice.Device.Driver.IsCardReaderOrCodeReader)
						guardZoneDevice.Device.ChildDescriptors.Add(guardZone);
					if (guardZoneDevice.Device.Driver.IsCardReaderOrCodeReader)
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
				mpt.ChildDescriptors.AddRange(mpt.MptLogic.GetObjects());
				foreach (var mptDevice in mpt.MPTDevices)
				{
					if (mptDevice.Device != null)
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
				}
				gkBases.Add(mpt);
			}

			foreach (var door in Doors)
			{
				door.ChildDescriptors.AddRange(door.OpenRegimeLogic.GetObjects());
				door.ChildDescriptors.AddRange(door.NormRegimeLogic.GetObjects());
				door.ChildDescriptors.AddRange(door.CloseRegimeLogic.GetObjects());

				if (door.EnterDevice != null)
				{
					door.ChildDescriptors.Add(door.EnterDevice);
				}

				if (door.ExitDevice != null)
				{
					door.ChildDescriptors.Add(door.ExitDevice);
				}

				if (door.EnterButton != null)
				{
					door.ChildDescriptors.Add(door.EnterButton);
				}

				if (door.ExitButton != null)
				{
					door.ChildDescriptors.Add(door.ExitButton);
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
				}

				if (door.LockControlDeviceExit != null)
				{
					door.ChildDescriptors.Add(door.LockControlDeviceExit);
				}

				gkBases.Add(door);
			}

			foreach (var device in Devices)
			{
				device.ChildDescriptors.AddRange(device.Logic.GetObjects());
				if (device.Driver.HasMirror)
				{
					switch (device.DriverType)
					{
						case GKDriverType.DetectorDevicesMirror:
							device.GKReflectionItem.Devices.ForEach(x => device.ChildDescriptors.Add(x));
							break;

						case GKDriverType.ControlDevicesMirror:
							device.GKReflectionItem.Devices.ForEach(x => { x.ChildDescriptors.Add(device); device.ChildDescriptors.Add(x); });
							device.GKReflectionItem.Diretions.ForEach(x => { x.ChildDescriptors.Add(device); device.ChildDescriptors.Add(x); });
							device.GKReflectionItem.Delays.ForEach(x => { x.ChildDescriptors.Add(device); device.ChildDescriptors.Add(x); });
							device.GKReflectionItem.MPTs.ForEach(x => { x.ChildDescriptors.Add(device); device.ChildDescriptors.Add(x); });
							device.GKReflectionItem.NSs.ForEach(x => { x.ChildDescriptors.Add(device); device.ChildDescriptors.Add(x); });
							break;
						case GKDriverType.DirectionsMirror:
							device.GKReflectionItem.Diretions.ForEach(x => { x.ChildDescriptors.Add(device); device.ChildDescriptors.Add(x); });
							break;
						case GKDriverType.FireZonesMirror:
							device.GKReflectionItem.Zones.ForEach(x => { x.ChildDescriptors.Add(device); device.ChildDescriptors.Add(x); });
							break;
						case GKDriverType.FirefightingZonesMirror:
							device.GKReflectionItem.Zones.ForEach(x => { x.ChildDescriptors.Add(device); device.ChildDescriptors.Add(x); });
							device.GKReflectionItem.Diretions.ForEach(x => { x.ChildDescriptors.Add(device); device.ChildDescriptors.Add(x); });
							break;
						case GKDriverType.GuardZonesMirror:
							device.GKReflectionItem.GuardZones.ForEach(x => { x.ChildDescriptors.Add(device); device.ChildDescriptors.Add(x); });
							break;
					}
				}
				if (device.DriverType == GKDriverType.RSR2_MAP4)
				{
					device.ChildDescriptors.AddRange(new List<GKBase>(device.Zones));
				}
				gkBases.Add(device);
			}

			gkBases.ForEach(x => x.ClearDescriptor());
			gkBases.ForEach(x => x.PrepareInputOutputDependences());
			gkBases.ForEach(x => x.Invalidate(this));
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

				if (gkBase is GKMPT)
				{
					var mpt = gkBase as GKMPT;
					if (mpt.HasAccessLevel)
					{
						gkBase.KauParents.Clear();
						gkBase.MagnetToGK = true;
					}
				}

				if (gkBase is GKGuardZone)
				{
					var guardZone = gkBase as GKGuardZone;
					if (guardZone.HasAccessLevel)
					{
						gkBase.KauParents.Clear();
						gkBase.MagnetToGK = true;
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