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
            if ((deviceState != null) && (deviceState.Device.Driver.CanDisable))
            {
                if (deviceState.IsDisabled)
                    return FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_RemoveFromIgnoreList);
                return FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_AddToIgnoreList);
            }
            return false;
        }

        public static void ChangeDisabled(this DeviceState deviceState)
        {
            if ((deviceState != null) && (deviceState.CanDisable()))
            {
                if (deviceState.IsDisabled)
                    FiresecManager.FiresecService.RemoveFromIgnoreList(new List<Guid>() { deviceState.Device.UID });
                else
                    FiresecManager.FiresecService.AddToIgnoreList(new List<Guid>() { deviceState.Device.UID });
            }
        }
    }
}