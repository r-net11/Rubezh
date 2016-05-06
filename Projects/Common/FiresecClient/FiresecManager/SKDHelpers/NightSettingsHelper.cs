using System;
using StrazhAPI.SKD;

namespace FiresecClient.SKDHelpers
{
	public static class NightSettingsHelper
	{
		public static NightSettings GetByOrganisation(Guid uid)
		{
			var result = FiresecManager.FiresecService.GetNightSettingsByOrganisation(uid);
			return result.HasError ? null : result.Result;
		}

		public static bool Save(NightSettings item)
		{
			return Common.ShowErrorIfExists(FiresecManager.FiresecService.SaveNightSettings(item));
		}
	}
}