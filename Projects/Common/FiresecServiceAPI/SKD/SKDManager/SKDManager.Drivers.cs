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
			systemDriver.Children.Add(SKDDriverType.Controller);
			Drivers.Add(systemDriver);

			var controllerDriver = new SKDDriver()
			{
				UID = new Guid("3D8FEF42-BAF6-422D-9A4A-E6EF0072896D"),
				Name = "Контроллер",
				ShortName = "Контроллер",
				DriverType = SKDDriverType.Controller,
				CanEditAddress = true,
				IsControlDevice = true,
				HasZone = true,
				IsPlaceable = true
			};
			controllerDriver.Children.Add(SKDDriverType.Reader);
			controllerDriver.Children.Add(SKDDriverType.Gate);
			controllerDriver.AvailableStateClasses.Add(XStateClass.Norm);
			controllerDriver.AvailableStateClasses.Add(XStateClass.Failure);
			controllerDriver.AvailableStateClasses.Add(XStateClass.Unknown);

			var driverProperty = new XDriverProperty()
			{
				Name = "Port",
				Caption = "Порт",
				DriverPropertyType = XDriverPropertyTypeEnum.IntType,
				Min = 10000,
				Max = 20000,
				Default = 10000
			};
			controllerDriver.Properties.Add(driverProperty);
			Drivers.Add(controllerDriver);

			var readerDriver = new SKDDriver()
			{
				UID = new Guid("E54EC896-178E-46F0-844F-0B8BE1770F44"),
				Name = "Считыватель",
				ShortName = "Считыватель",
				DriverType = SKDDriverType.Reader,
				CanEditAddress = false,
				HasZone = true,
				HasOuterZone = true,
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
				HasOuterZone = true,
				IsPlaceable = true
			};
			gateDriver.AvailableStateClasses.Add(XStateClass.Norm);
			gateDriver.AvailableStateClasses.Add(XStateClass.Failure);
			gateDriver.AvailableStateClasses.Add(XStateClass.Unknown);
			Drivers.Add(gateDriver);
		}
	}
}