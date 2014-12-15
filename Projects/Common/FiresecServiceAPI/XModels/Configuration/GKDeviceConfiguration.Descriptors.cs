using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using System;

namespace FiresecAPI.GK
{
	public partial class GKDeviceConfiguration
	{
		public void PrepareDescriptors()
		{
			GKBases.ForEach(x => x.ClearDescriptor());
			Devices.ForEach(x => x.ClearDescriptor());
			PrepareDevices();
			PrepareObjects();
			PrepareCodes();
			PrepareDoors();
		}

		List<GKBase> GKBases
		{
			get
			{
				var gkBases = new List<GKBase>();
				gkBases.AddRange(Zones);
				gkBases.AddRange(Directions);
				gkBases.AddRange(PumpStations);
				gkBases.AddRange(MPTs);
				gkBases.AddRange(Delays);
				gkBases.AddRange(GuardZones);
				return gkBases;
			}
		}

		void InitializeDataBaseParent(GKBase gkBase)
		{
			gkBase.KauDatabaseParent = null;
			gkBase.GkDatabaseParent = null;

			var dataBaseParent = gkBase.GetDataBaseParent();
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
			foreach (var gkBase in GKBases)
				InitializeDataBaseParent(gkBase);
		}

		void PrepareDevices()
		{
			foreach (var device in Devices)
			{
				var dataBaseParent = device.GetDataBaseParent();
				if (dataBaseParent == null)
					continue;
				device.IsLogicOnKau = dataBaseParent.Driver.IsKauOrRSR2Kau;
				if (device.Door != null && device.Door.LockDeviceUID == device.UID)
				{
					device.IsLogicOnKau = false;
				}
			}
		}

		void PrepareCodes()
		{
			foreach (var code in Codes)
			{
				code.PrepareInputOutputDependences();
				var codeGuardZones = new List<GKGuardZone>();
				foreach (var guardZone in GKManager.GuardZones)
				{
					var codeUids = new List<Guid>();
					foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
					{
						codeUids.AddRange(guardZoneDevice.CodeReaderSettings.SetGuardSettings.CodeUIDs);
						codeUids.AddRange(guardZoneDevice.CodeReaderSettings.ResetGuardSettings.CodeUIDs);
						codeUids.AddRange(guardZoneDevice.CodeReaderSettings.ChangeGuardSettings.CodeUIDs);
						codeUids.AddRange(guardZoneDevice.CodeReaderSettings.AlarmSettings.CodeUIDs);
					}
					if (codeUids.Contains(code.UID))
						codeGuardZones.Add(guardZone);
				}
				List<GKDevice> kauParents = codeGuardZones.Select(x => x.KauDatabaseParent).ToList();
				if (kauParents != null && kauParents.Count == 1)
					code.KauDatabaseParent = kauParents.FirstOrDefault();
				else
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
	}
}