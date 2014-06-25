using System;
using System.Collections.Generic;
using FiresecAPI.GK;

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
				CanEditAddress = false,
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

			var readerDriver = new SKDDriver()
			{
				UID = new Guid("E54EC896-178E-46F0-844F-0B8BE1770F44"),
				Name = "Считыватель",
				ShortName = "Считыватель",
				DriverType = SKDDriverType.Reader,
				CanEditAddress = false,
				HasZone = true,
				IsPlaceable = true
			};
			readerDriver.AvailableStateClasses.Add(XStateClass.Norm);
			readerDriver.AvailableStateClasses.Add(XStateClass.Failure);
			readerDriver.AvailableStateClasses.Add(XStateClass.Unknown);
			Drivers.Add(readerDriver);

			var gateDriver = new SKDDriver()
			{
				UID = new Guid("464B4AC5-B192-4890-9605-C41C6E3C883B"),
				Name = "Шлакбаум",
				ShortName = "Шлакбаум",
				DriverType = SKDDriverType.Gate,
				CanEditAddress = false,
				HasZone = true,
				IsPlaceable = true
			};
			gateDriver.AvailableStateClasses.Add(XStateClass.Norm);
			gateDriver.AvailableStateClasses.Add(XStateClass.Failure);
			gateDriver.AvailableStateClasses.Add(XStateClass.Unknown);
			Drivers.Add(gateDriver);
		}
	}
}