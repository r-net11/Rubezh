using System;
using StrazhAPI.GK;

namespace StrazhAPI.SKD
{
	public static class ChinaController_2_Helper
	{
		public static SKDDriver Create()
		{
			var driver = new SKDDriver
			{
				UID = new Guid("55275074-0665-4951-8A0D-85BF51533B59"),
                Name = Resources.Language.SKD.SKDManager.ChinaController_2_Helper.Name,
                ShortName = Resources.Language.SKD.SKDManager.ChinaController_2_Helper.ShortName,
				DriverType = SKDDriverType.ChinaController_2,
				IsPlaceable = true
			};
			driver.Children.Add(SKDDriverType.Reader);
			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Failure);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);

			driver.AutocreationItems.Add(new SKDDriverAutocreationItem(SKDDriverType.Reader, 2));
			driver.AutocreationItems.Add(new SKDDriverAutocreationItem(SKDDriverType.Lock, 2));
			driver.AutocreationItems.Add(new SKDDriverAutocreationItem(SKDDriverType.LockControl, 2));
			driver.AutocreationItems.Add(new SKDDriverAutocreationItem(SKDDriverType.Button, 2));

			driver.Properties = CommonChinaControllerHelper.CreateProperties();
			return driver;
		}
	}
}