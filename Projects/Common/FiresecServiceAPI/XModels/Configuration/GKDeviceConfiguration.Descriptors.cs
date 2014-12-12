using System.Collections.Generic;
using System.Linq;
using FiresecClient;

namespace FiresecAPI.GK
{
	public partial class GKDeviceConfiguration
	{
		public void PrepareDescriptors()
		{
			PrepareDevices();
			PrepareObjects();
			PrepareCodes();
			PrepareDoors();
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