using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.GK;

namespace FiresecAPI.SKD
{
	public static class ChinaController_4_4Helper
	{
		public static SKDDriver Create()
		{
			var driver = new SKDDriver()
			{
				UID = new Guid("48A0C16E-DEA6-40d2-98B9-A21076BF6F37"),
				Name = "Контроллер на четыре двери и четыре считывателя",
				ShortName = "Контроллер",
				DriverType = SKDDriverType.ChinaController_4_4,
				CanEditAddress = true,
				IsControlDevice = true,
				IsPlaceable = true,
				DoorsCount = 4,
				ReadersCount = 4
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