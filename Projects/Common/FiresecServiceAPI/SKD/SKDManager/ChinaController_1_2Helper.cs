using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.GK;

namespace FiresecAPI.SKD
{
	public static class ChinaController_1_2Helper
	{
		public static SKDDriver Create()
		{
			var driver = new SKDDriver()
			{
				UID = new Guid("3D8FEF42-BAF6-422D-9A4A-E6EF0072896D"),
				Name = "Контроллер на одну дверь и два считывателя",
				ShortName = "Контроллер",
				DriverType = SKDDriverType.ChinaController_1_2,
				IsControlDevice = true,
				IsPlaceable = true
			};
			driver.Children.Add(SKDDriverType.Reader);
			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Failure);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);

			driver.AutocreationItems.Add(new SKDDriverAutocreationItem(SKDDriverType.Reader, 2));
			driver.AutocreationItems.Add(new SKDDriverAutocreationItem(SKDDriverType.Lock, 1));
			driver.AutocreationItems.Add(new SKDDriverAutocreationItem(SKDDriverType.LockControl, 1));
			driver.AutocreationItems.Add(new SKDDriverAutocreationItem(SKDDriverType.Button, 1));

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