using System;
using System.Collections.Generic;
using Common;
using FiresecAPI.Models;

namespace FiresecClient
{
    public partial class FiresecManager
    {
        public static bool CanDisable(DeviceState deviceState)
        {
            try
            {
                if ((deviceState != null) && (deviceState.Device.Driver.CanDisable))
                {
                    if (deviceState.IsDisabled)
                        return CheckPermission(PermissionType.Oper_RemoveFromIgnoreList);
                    return CheckPermission(PermissionType.Oper_AddToIgnoreList);
                }
                return false;
            }
            catch (Exception e)
            {
                Logger.Error(e, "FiresecManager.CanDisable");
                return false;
            }
        }

        public static void ChangeDisabled(DeviceState deviceState)
        {
            try
            {
                if ((deviceState != null) && (CanDisable(deviceState)))
                {
                    if (deviceState.IsDisabled)
                        RemoveFromIgnoreList(new List<Device>() { deviceState.Device });
                    else
                        AddToIgnoreList(new List<Device>() { deviceState.Device });
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "FiresecManager.ChangeDisabled");
            }
        }
    }
}