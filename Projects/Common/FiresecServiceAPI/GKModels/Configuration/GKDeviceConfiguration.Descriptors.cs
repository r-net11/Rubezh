using System.Collections.Generic;
using System.Diagnostics;
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
			GKBases.ForEach(x => { InitializeDataBaseParent(x); });
			PrepareCodes();
			PrepareDoors();

			SetDependentDescriptors();
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
		}

		void PrepareDevices()
		{
			foreach (var device in Devices)
			{
				device.KauDatabaseParent = device.KAUParent;
				if (device.DataBaseParent != null)
				{
					if (device.Door != null && device.Door.LockDeviceUID == device.UID)
					{
						device.IsLogicOnKau = false;
					}
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