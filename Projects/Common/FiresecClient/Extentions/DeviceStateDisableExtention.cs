using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

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
                    return FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_RemoveFromIgnoreList);
                }
                else
                {
                    return FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_AddToIgnoreList);
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