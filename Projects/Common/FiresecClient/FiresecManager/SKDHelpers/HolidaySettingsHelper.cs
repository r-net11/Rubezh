using System;
using FiresecAPI.SKD;

namespace FiresecClient.SKDHelpers
{
    public static class HolidaySettingsHelper
    {
        public static HolidaySettings GetByOrganisation(Guid uid)
        {
            return Common.ShowErrorIfExists(FiresecManager.FiresecService.GetHolidaySettingsByOrganisation(uid));
        }

        public static bool Save(HolidaySettings item)
        {
            return Common.ShowErrorIfExists(FiresecManager.FiresecService.SaveHolidaySettings(item));
        }
    }
}
