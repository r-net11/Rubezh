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
			var value = RegistrySettingsHelper.Get(applicationName);
			if (value != null)
			{
				try
				{
					return int.Parse(value);
				}
				catch { }
			}
			return 0;
		}

		public static void SetPatchNo(string applicationName, int value)
		{
			RegistrySettingsHelper.Set(applicationName, value.ToString());
		}
	}
}