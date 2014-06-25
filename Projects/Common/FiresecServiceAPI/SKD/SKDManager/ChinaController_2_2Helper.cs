using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.GK;

namespace FiresecAPI.SKD
{
	public static class ChinaController_2_2Helper
	{
		public static SKDDriver Create()
		{
			var driver = new SKDDriver()
			{
				UID = new Guid("55275074-0665-4951-8A0D-85BF51533B59"),
				Name = "Контроллер на две двери и два считывателя",
				ShortName = "Контроллер",
				DriverType = SKDDriverType.ChinaController_2_2,
				CanEditAddress = true,
				IsControlDevice = true,
				IsPlaceable = true,
				DoorsCount = 2,
				ReadersCount = 2
			};
			driver.Children.Add(SKDDriverType.Reader);
			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Failure);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);

			var addressProperty = new XDriverProperty()
			{
				Name = "Address",
				Caption = "Адрес",
				DriverPropertyType = XDriverPropertyTypeEnum.StringType,
				StringDefault = "192.168.0.1"
			};
			driver.Properties.Add(addressProperty);

			var driverProperty = new XDriverProperty()
			{
				Name = "Port",
				Caption = "Порт",
				DriverPropertyType = XDriverPropertyTypeEnum.IntType,
				Default = 37777
			};
			driver.Properties.Add(driverProperty);

			var loginProperty = new XDriverProperty()
			{
				Name = "Login",
				Caption = "Логин",
				DriverPropertyType = XDriverPropertyTypeEnum.StringType,
				StringDefault = "admin"
			};
			driver.Properties.Add(loginProperty);

			var passwordProperty = new XDriverProperty()
			{
				Name = "Password",
				Caption = "Пароль",
				DriverPropertyType = XDriverPropertyTypeEnum.StringType,
				StringDefault = "123456"
			};
			driver.Properties.Add(passwordProperty);

			return driver;
		}
	}
}