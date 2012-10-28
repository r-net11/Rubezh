using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		public static bool CanDisable(DeviceState deviceState)
		{
			if ((deviceState != null) && (deviceState.Device.Driver.CanDisable))
			{
				if (deviceState.IsDisabled)
					return CheckPermission(PermissionType.Oper_RemoveFromIgnoreList);
				return CheckPermission(PermissionType.Oper_AddToIgnoreList);
			}
			return false;
		}

		public static void ChangeDisabled(DeviceState deviceState)
		{
			if ((deviceState != null) && (CanDisable(deviceState)))
			{
				if (deviceState.IsDisabled)
					FiresecDriver.RemoveFromIgnoreList(new List<Device>() { deviceState.Device });
				else
					FiresecDriver.AddToIgnoreList(new List<Device>() { deviceState.Device });
			}
		}
	}
}