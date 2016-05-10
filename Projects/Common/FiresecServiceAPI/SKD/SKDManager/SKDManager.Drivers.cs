using System;
using System.Collections.Generic;

namespace StrazhAPI.SKD
{
	public partial class SKDManager
	{
		public static void CreateDrivers()
		{
			Drivers = new List<SKDDriver>();
			var systemDriver = new SKDDriver()
			{
				UID = new Guid("EC1089D1-7DBC-4797-A5AB-F37104999E91"),
                Name = Resources.Language.SKD.SKDManager.SKDManagerDrivers.Name,
                ShortName = Resources.Language.SKD.SKDManager.SKDManagerDrivers.ShortName,
				DriverType = SKDDriverType.System,
			};
			systemDriver.Children.Add(SKDDriverType.ChinaController_1);
			systemDriver.Children.Add(SKDDriverType.ChinaController_2);
			systemDriver.Children.Add(SKDDriverType.ChinaController_4);
			Drivers.Add(systemDriver);

			Drivers.Add(ChinaController_1_Helper.Create());
			Drivers.Add(ChinaController_2_Helper.Create());
			Drivers.Add(ChinaController_4_Helper.Create());
			Drivers.Add(ReaderHelper.Create());
			Drivers.Add(LockHelper.Create());
			Drivers.Add(LockControlHelper.Create());
			Drivers.Add(ButtonHelper.Create());
		}
	}
}