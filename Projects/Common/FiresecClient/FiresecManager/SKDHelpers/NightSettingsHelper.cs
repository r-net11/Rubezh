using System;
using FiresecAPI.SKD;

namespace FiresecClient.SKDHelpers
{
	public static class NightSettingsHelper
	{
		public static NightSettings GetByOrganisation(Guid uid)
		{
			return Common.ShowErrorIfExists(FiresecManager.FiresecService.GetNightSettingsByOrganisation(uid));
		}

		public static bool Save(NightSettings item)
		{
			return Common.ShowErrorIfExists(FiresecManager.FiresecService.SaveNightSettings(item));
		}
	}
}