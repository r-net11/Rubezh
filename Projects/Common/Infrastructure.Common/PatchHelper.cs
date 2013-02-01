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
			return RegistrySettingsHelper.GetInt(applicationName);
		}

		public static void SetPatchNo(string applicationName, int value)
		{
			RegistrySettingsHelper.SetInt(applicationName, value);
		}
	}
}