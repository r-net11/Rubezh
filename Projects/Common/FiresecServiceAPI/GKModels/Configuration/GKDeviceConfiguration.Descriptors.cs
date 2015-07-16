using System.Collections.Generic;
using System.Linq;
using FiresecClient;

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
			gkBase.IsLogicOnKau = dataBaseParent.Driver.IsKau;
			if (dataBaseParent.Driver.IsKau)
			{
				gkBase.KauDatabaseParent = dataBaseParent;
				gkBase.GkDatabaseParent = dataBaseParent.GKParent;
			}
			else
				gkBase.GkDatabaseParent = dataBaseParent;
			if (gkBase is GKGuardZone && (gkBase as GKGuardZone).HasAccessLevel)
			{
				gkBase.IsLogicOnKau = false;
				gkBase.KauDatabaseParent = null;
			}
			if (gkBase is GKMPT)
			{
				foreach (var mptDevice in (gkBase as GKMPT).MPTDevices)
				{
					mptDevice.Device.IsLogicOnKau = gkBase.IsLogicOnKau;
				}
			}
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
				device.IsLogicOnKau = dataBaseParent.Driver.IsKau && device.KAUParent == dataBaseParent;
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
				var codeGuardZones = GKManager.GuardZones.Where(x => x.GetCodeUids().Contains(code.UID)).ToList();
				var codeMPTs = GKManager.MPTs.Where(x => x.GetCodeUids().Contains(code.UID)).ToList();
				var allKauParents = codeGuardZones.Select(x => x.KauDatabaseParent).ToList();
				allKauParents.AddRange(codeMPTs.Select(x => x.KauDatabaseParent).ToList());
				var kauParents = allKauParents.Distinct().ToList();
				code.KauDatabaseParent = kauParents.Count == 1 ? kauParents.FirstOrDefault() : null;
				code.GkDatabaseParent = GKManager.Devices.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
				if (code.KauDatabaseParent == null)
				{
					codeMPTs.ForEach(x =>
					{
						x.KauDatabaseParent = code.KauDatabaseParent;
						x.IsLogicOnKau = false;
					});
				}
			}
		}

		void PrepareDoors()
		{
			foreach (var door in Doors)
			{
				door.PrepareInputOutputDependences();
				door.KauDatabaseParent = null;
				door.GkDatabaseParent = door.EnterDevice != null ? door.EnterDevice.GKParent : null;
			}
		}
	}
}