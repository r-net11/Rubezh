using RubezhAPI.SKD;
using System;

namespace RubezhClient.SKDHelpers
{
	public static class NightSettingsHelper
	{
		public static NightSettings GetByOrganisation(Guid uid)
		{
			return Common.ShowErrorIfExists(ClientManager.RubezhService.GetNightSettingsByOrganisation(uid));
		}

		public static bool Save(NightSettings item)
		{
			return Common.ShowErrorIfExists(ClientManager.RubezhService.SaveNightSettings(item));
		}
	}
}