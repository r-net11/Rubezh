using XFiresecAPI;

namespace FiresecClient
{
	public partial class XManager
	{
		public static bool CanDisable(XDeviceState deviceState)
		{
			return true;
			//if ((deviceState != null) && (deviceState.Device.Driver.CanDisable))
			//{
			//    if (deviceState.IsDisabled)
			//        return CurrentUser.Permissions.Any(x => x == PermissionType.Oper_RemoveFromIgnoreList);
			//    return CurrentUser.Permissions.Any(x => x == PermissionType.Oper_AddToIgnoreList);
			//}
			//return false;
		}

		public static void ChangeDisabled(XDeviceState deviceState)
		{
			if ((deviceState != null) && (CanDisable(deviceState)))
			{
				if (deviceState.IsDisabled)
				{
					//FiresecService.RemoveFromIgnoreList(new List<Guid>() { deviceState.Device.UID });
				}
				else
				{
					//FiresecService.AddToIgnoreList(new List<Guid>() { deviceState.Device.UID });
				}
			}
		}
	}
}