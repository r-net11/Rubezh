using System;
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
				IsPlaceable = true
			};
			driver.Children.Add(SKDDriverType.Reader);
			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Failure);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);

			driver.AutocreationItems.Add(new SKDDriverAutocreationItem(SKDDriverType.Reader, 4));
			driver.AutocreationItems.Add(new SKDDriverAutocreationItem(SKDDriverType.Lock, 4));
			driver.AutocreationItems.Add(new SKDDriverAutocreationItem(SKDDriverType.LockControl, 4));
			driver.AutocreationItems.Add(new SKDDriverAutocreationItem(SKDDriverType.Button, 4));

			var addressProperty = new SKDDriverProperty()
			{
				Name = "Address",
				Caption = "Адрес",
				DriverPropertyType = XDriverPropertyTypeEnum.StringType,
				StringDefault = "192.168.0.1"
			};
			driver.Properties.Add(addressProperty);

			var driverProperty = new SKDDriverProperty()
			{
				Name = "Port",
				Caption = "Порт",
				DriverPropertyType = XDriverPropertyTypeEnum.IntType,
				Default = 37777
			};
			driver.Properties.Add(driverProperty);

			var loginProperty = new SKDDriverProperty()
			{
				Name = "Login",
				Caption = "Логин",
				DriverPropertyType = XDriverPropertyTypeEnum.StringType,
				StringDefault = "admin"
			};
			driver.Properties.Add(loginProperty);

			var passwordProperty = new SKDDriverProperty()
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