using System.Collections.Generic;
using System.Linq;
using FiresecClient;

namespace FiresecAPI.GK
{
	public partial class GKDeviceConfiguration
	{
		public void PrepareDescriptors()
		{
			PrepareInputOutputDependences();
			PrepareDevices();
			PrepareObjects();
			PrepareCodes();
			PrepareDoors();
		}

		GKDevice GetDataBaseParent(List<GKDevice> devices)
		{
			var allDependentDevices = new List<GKDevice>(devices);
			foreach (var device in devices)
			{

			}
			List<GKDevice> kauParents = allDependentDevices.Select(x => x.KAUParent).ToList();
			if (kauParents != null && kauParents.Count == 1)
				return kauParents.FirstOrDefault();
			else
				return devices.FirstOrDefault().GKParent;
		}

		GKDevice GetDataBaseParent(GKBase gkBase)
		{
			var allDependentDevices = GetFullTree(gkBase);
			var kauParents = allDependentDevices.Select(x => x.KAUParent).Distinct().ToList();
			if (kauParents != null && kauParents.Count == 1 && kauParents.FirstOrDefault() != null)
				return kauParents.FirstOrDefault();
			else
			{
				if (gkBase is GKDevice)
					return (gkBase as GKDevice).GKParent;
				else
				{
					if (allDependentDevices != null && allDependentDevices.Count > 0)
						return allDependentDevices.FirstOrDefault().GKParent;
					else
						return null;
				}
			}				
		}

		List<GKDevice> GetFullTree(GKBase gkBase)
		{
			return GetAllDependentObjects(gkBase).Where(x => x is GKDevice).Cast<GKDevice>().ToList();
		}

		List<GKBase> GetAllDependentObjects(GKBase gkBase)
		{
			var result = new List<GKBase>();
			var inputObjects = gkBase.InputGKBases;
			inputObjects.RemoveAll(x => x == gkBase);
			result.AddRange(gkBase.InputGKBases);
			foreach (var inputObject in inputObjects)
				result.AddRange(GetAllDependentObjects(inputObject));
			return result;
		}

		void InitializeDataBaseParent(GKBase gkBase)
		{
			gkBase.KauDatabaseParent = null;
			gkBase.GkDatabaseParent = null;

			var dataBaseParent = GetDataBaseParent(gkBase);
			if (dataBaseParent == null)
				return;
			gkBase.IsLogicOnKau = dataBaseParent.Driver.IsKauOrRSR2Kau;
			if (dataBaseParent.Driver.IsKauOrRSR2Kau)
			{
				gkBase.KauDatabaseParent = dataBaseParent;
				gkBase.GkDatabaseParent = dataBaseParent.GKParent;
			}
			else
				gkBase.GkDatabaseParent = dataBaseParent;
		}

		void PrepareObjects()
		{
			var gkBases = new List<GKBase>();
			gkBases.AddRange(Zones);
			gkBases.AddRange(Directions);
			gkBases.AddRange(PumpStations);
			gkBases.AddRange(MPTs);
			gkBases.AddRange(Delays);
			gkBases.AddRange(GuardZones);
			foreach (var gkBase in gkBases)
				InitializeDataBaseParent(gkBase);
		}

		void PrepareDevices()
		{
			foreach (var device in Devices)
			{
				var dataBaseParent = GetDataBaseParent(device);
				if (dataBaseParent == null)
					continue;
				device.IsLogicOnKau = dataBaseParent.Driver.IsKauOrRSR2Kau;
			}
		}

		void PrepareCodes()
		{
			foreach (var code in Codes)
			{
				code.KauDatabaseParent = null;
				code.GkDatabaseParent = GKManager.Devices.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
			}
		}

		void PrepareDoors()
		{
			foreach (var door in Doors)
			{
				door.KauDatabaseParent = null;
				door.GkDatabaseParent = door.EnterDevice != null ? door.EnterDevice.GKParent : null;
			}
		}

		void PrepareInputOutputDependences()
		{
			foreach (var device in Devices)
			{
				device.ClearDescriptor();
			}
			foreach (var zone in Zones)
			{
				zone.ClearDescriptor();
			}
			foreach (var direction in Directions)
			{
				direction.ClearDescriptor();
			}
			foreach (var pumpStation in PumpStations)
			{
				pumpStation.ClearDescriptor();
			}
			foreach (var mpt in MPTs)
			{
				mpt.ClearDescriptor();
			}
			foreach (var delay in Delays)
			{
				delay.ClearDescriptor();
			}
			foreach (var guardZone in GuardZones)
			{
				guardZone.ClearDescriptor();
			}
			foreach (var code in Codes)
			{
				code.ClearDescriptor();
			}
			foreach (var door in Doors)
			{
				door.ClearDescriptor();
			}

			foreach (var device in Devices)
			{
				LinkLogic(device, device.Logic.OnClausesGroup);
				LinkLogic(device, device.Logic.OffClausesGroup);
				LinkLogic(device, device.Logic.StopClausesGroup);
			}

			foreach (var zone in Zones)
			{
				foreach (var device in zone.Devices)
				{
					LinkGKBases(zone, device);
				}
				LinkGKBases(zone, zone);
			}

			foreach (var direction in Directions)
			{
				LinkLogic(direction, direction.Logic.OnClausesGroup);
				LinkLogic(direction, direction.Logic.OffClausesGroup);
			}

			foreach (var pumpStation in PumpStations)
			{
				LinkLogic(pumpStation, pumpStation.StartLogic.OnClausesGroup);
				LinkLogic(pumpStation, pumpStation.StopLogic.OnClausesGroup);
				LinkLogic(pumpStation, pumpStation.AutomaticOffLogic.OnClausesGroup);
			}

			foreach (var mpt in MPTs)
			{
				LinkLogic(mpt, mpt.StartLogic.OnClausesGroup);
			}

			foreach (var delay in Delays)
			{
				LinkLogic(delay, delay.Logic.OnClausesGroup);
				LinkLogic(delay, delay.Logic.OffClausesGroup);
			}

			foreach (var guardZone in GuardZones)
			{
				foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
				{
					if (guardZoneDevice.ActionType != GKGuardZoneDeviceActionType.ChangeGuard)
					{
						LinkGKBases(guardZone, guardZoneDevice.Device);
						if (guardZoneDevice.Device.DriverType == GKDriverType.RSR2_GuardDetector || guardZoneDevice.Device.DriverType == GKDriverType.RSR2_CodeReader)
						{
							LinkGKBases(guardZoneDevice.Device, guardZone);
						}
					}
				}
				LinkGKBases(guardZone, guardZone);
			}

			foreach (var door in Doors)
			{
				if (door.EnterDevice != null)
					LinkGKBases(door, door.EnterDevice);
				if (door.ExitDevice != null)
					LinkGKBases(door, door.ExitDevice);
				if (door.LockDevice != null)
					LinkGKBases(door.LockDevice, door);
				if (door.LockControlDevice != null)
					LinkGKBases(door, door.LockControlDevice);
			}
		}

		void LinkLogic(GKBase gkBase, GKClauseGroup clauseGroup)
		{
			if (clauseGroup.Clauses != null)
			{
				foreach (var clause in clauseGroup.Clauses)
				{
					foreach (var clauseDevice in clause.Devices)
						LinkGKBases(gkBase, clauseDevice);
					foreach (var zone in clause.Zones)
						LinkGKBases(gkBase, zone);
					foreach (var guardZone in clause.GuardZones)
						LinkGKBases(gkBase, guardZone);
					foreach (var direction in clause.Directions)
						LinkGKBases(gkBase, direction);
					foreach (var mpt in clause.MPTs)
						LinkGKBases(gkBase, mpt);
					foreach (var delay in clause.Delays)
						LinkGKBases(gkBase, delay);
				}
			}
			if (clauseGroup.ClauseGroups != null)
			{
				foreach (var group in clauseGroup.ClauseGroups)
				{
					LinkLogic(gkBase, group);
				}
			}
		}

		public static void LinkGKBases(GKBase value, GKBase dependsOn)
		{
			AddInputOutputObject(value.InputGKBases, dependsOn);
			AddInputOutputObject(dependsOn.OutputGKBases, value);
		}

		static void AddInputOutputObject(List<GKBase> objects, GKBase newObject)
		{
			if (!objects.Any(x => x.UID == newObject.UID))
				objects.Add(newObject);
		}
	}
}