using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace FiresecAPI
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
				DriverType = SKDDriverType.System
			};
			systemDriver.Children.Add(SKDDriverType.Controller);
			Drivers.Add(systemDriver);

			var controllerDriver = new SKDDriver()
			{
				UID = new Guid("3D8FEF42-BAF6-422D-9A4A-E6EF0072896D"),
				Name = "Контроллер",
				ShortName = "Контроллер",
				DriverType = SKDDriverType.Controller
			};
			controllerDriver.Children.Add(SKDDriverType.Reader);
			controllerDriver.Children.Add(SKDDriverType.Gate);
			Drivers.Add(controllerDriver);

			var readerDriver = new SKDDriver()
			{
				UID = new Guid("E54EC896-178E-46F0-844F-0B8BE1770F44"),
				Name = "Считыватель",
				ShortName = "Считыватель",
				DriverType = SKDDriverType.Reader
			};
			Drivers.Add(readerDriver);

			var gateDriver = new SKDDriver()
			{
				UID = new Guid("464B4AC5-B192-4890-9605-C41C6E3C883B"),
				Name = "Шлакбаум",
				ShortName = "Шлакбаум",
				DriverType = SKDDriverType.Gate
			};
			Drivers.Add(gateDriver);
		}
	}
}