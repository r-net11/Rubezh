using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using System;

namespace FiresecClient
{
    public static class DeviceStateDisableExtention
    {
        public static bool CanDisable(this DeviceState deviceState)
        {
            if (deviceState.Device.Driver.CanDisable)
            {
                if (deviceState.IsDisabled)
                {
                    return FiresecManager.CurrentPermissions.Any(x => x.PermissionType == PermissionType.Oper_RemoveFromIgnoreList);
                }
                else
                {
                    return FiresecManager.CurrentPermissions.Any(x => x.PermissionType == PermissionType.Oper_AddToIgnoreList);
                }
            }
            return false;
        }

        public static void ChangeDisabled(this DeviceState deviceState)
        {
            if (deviceState.CanDisable())
            {
                if (deviceState.IsDisabled)
                {
                    FiresecManager.RemoveFromIgnoreList(new List<Guid>() { deviceState.Device.UID });
                }
                else
                {
                    FiresecManager.AddToIgnoreList(new List<Guid>() { deviceState.Device.UID });
                }
            }
        }
    }
}