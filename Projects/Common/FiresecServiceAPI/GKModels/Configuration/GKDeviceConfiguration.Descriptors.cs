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
			Devices.ForEach(x => x.ClearDescriptor());
			PrepareDevices();
			PrepareDoors();
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