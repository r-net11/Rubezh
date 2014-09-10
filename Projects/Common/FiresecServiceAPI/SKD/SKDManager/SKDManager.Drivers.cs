using System;
using System.Collections.Generic;

namespace FiresecAPI.SKD
{
	public partial class SKDManager
	{
		public static void CreateDrivers()
		{
			Drivers = new List<SKDDriver>();
			var systemDriver = new SKDDriver()
			{
				UID = new Guid("EC1089D1-7DBC-4797-A5AB-F37104999E91"),
				Name = "Система",
				ShortName = "Система",
				DriverType = SKDDriverType.System,
			};
			systemDriver.Children.Add(SKDDriverType.ChinaController_1_2);
			systemDriver.Children.Add(SKDDriverType.ChinaController_2_2);
			systemDriver.Children.Add(SKDDriverType.ChinaController_2_4);
			systemDriver.Children.Add(SKDDriverType.ChinaController_4_4);
			Drivers.Add(systemDriver);

			Drivers.Add(ChinaController_1_2Helper.Create());
			Drivers.Add(ChinaController_2_2Helper.Create());
			Drivers.Add(ChinaController_2_4Helper.Create());
			Drivers.Add(ChinaController_4_4Helper.Create());
			Drivers.Add(ReaderHelper.Create());
			Drivers.Add(GateHelper.Create());
			Drivers.Add(LockHelper.Create());
			Drivers.Add(LockControlHelper.Create());
			Drivers.Add(ButtonHelper.Create());
		}
	}
}