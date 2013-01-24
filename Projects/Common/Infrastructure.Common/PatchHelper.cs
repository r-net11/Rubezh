using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Infrastructure.Common
{
	public static class PatchHelper
	{
		public static int GetPatchNo(string applicationName)
		{
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("software\\rubezh\\Firesec-2\\Patch");
			if (registryKey != null)
			{
				var value = registryKey.GetValue(applicationName);
				if (value != null)
				{
					return (int)value;
				}
			}
			return 0;
		}

		public static void SetPatchNo(string applicationName, int value)
		{
            RegistryKey registryKey = UACHelper.CreateSubKey("software\\rubezh\\Firesec-2\\Patch");
			if (registryKey != null)
			{
				registryKey.SetValue(applicationName, value);
			}
		}
	}
}