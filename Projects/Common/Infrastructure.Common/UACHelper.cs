using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using Microsoft.Win32;
using System.Security.AccessControl;

namespace Infrastructure.Common
{
    public static class UACHelper
    {
        public static bool IsAdministrator
        {
            get
            {
                var windowsIdentity = WindowsIdentity.GetCurrent();
                var windowsPrincipal = new WindowsPrincipal(windowsIdentity);
                return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public static RegistryKey CreateSubKey(string name)
        {
            if (!IsAdministrator)
                return null;

            string user = Environment.UserDomainName + "\\" + Environment.UserName;
            var registrySecurity = new RegistrySecurity();
            registrySecurity.AddAccessRule(new RegistryAccessRule(user,
                RegistryRights.WriteKey | RegistryRights.ChangePermissions,
                InheritanceFlags.None, PropagationFlags.None, AccessControlType.Deny));

            try
            {
                RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(name, RegistryKeyPermissionCheck.Default, registrySecurity);
                return registryKey;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }
    }
}