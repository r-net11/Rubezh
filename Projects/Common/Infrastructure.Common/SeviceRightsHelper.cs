﻿using Common;
using ProcessPrivileges;
using System;
using System.Diagnostics;

namespace Infrastructure.Common
{
	public static class SeviceRightsHelper
	{
		public static void Enable()
		{
			try
			{
				using (AccessTokenHandle accessTokenHandle =
					Process.GetCurrentProcess().GetAccessTokenHandle(
						TokenAccessRights.AdjustPrivileges | TokenAccessRights.Query))
				{
					AdjustPrivilegeResult backupResult = accessTokenHandle.EnablePrivilege(Privilege.CreateGlobal);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "SeviceRightsHelper.Enable");
			}
		}

		private static void Enable2()
		{
			try
			{
				Type privilegeType = Type.GetType("System.Security.AccessControl.Privilege");
				object privilege = Activator.CreateInstance(privilegeType, "SeCreateGlobalPrivilege");
				privilegeType.GetMethod("Enable").Invoke(privilege, null);
			}
			catch (Exception e)
			{
				Logger.Error(e, "SeviceRightsHelper.Enable2");
			}
		}

		private static void Disable2()
		{
			try
			{
				Type privilegeType = Type.GetType("System.Security.AccessControl.Privilege");
				object privilege = Activator.CreateInstance(privilegeType, "SeCreateGlobalPrivilege");
				privilegeType.GetMethod("Revert").Invoke(privilege, null);
			}
			catch (Exception e)
			{
				Logger.Error(e, "SeviceRightsHelper.Disable2");
			}
		}
	}
}