
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