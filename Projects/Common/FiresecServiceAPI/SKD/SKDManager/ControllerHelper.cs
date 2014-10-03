using System;
using FiresecAPI.GK;

namespace FiresecAPI.SKD
{
	public static class ControllerHelper
	{
		public static SKDDriver Create()
		{
			var driver = new SKDDriver()
			{
				UID = new Guid("93AFFD73-ACA8-421e-BA5B-1E5D3E5113B4"),
				Name = "Контроллер СКД",
				ShortName = "Контроллер",
				DriverType = SKDDriverType.Controller,
				IsPlaceable = true
			};
			driver.Children.Add(SKDDriverType.Reader);
			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Failure);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);

			var addressProperty = new SKDDriverProperty()
			{
				Name = "Address",
				Caption = "Адрес",
				DriverPropertyType = GKDriverPropertyTypeEnum.StringType,
				StringDefault = "192.168.0.1"
			};
			driver.Properties.Add(addressProperty);

			var driverProperty = new SKDDriverProperty()
			{
				Name = "Port",
				Caption = "Порт",
				DriverPropertyType = GKDriverPropertyTypeEnum.IntType,
				Min = 10000,
				Max = 20000,
				Default = 10000
			};
			driver.Properties.Add(driverProperty);

			var loginProperty = new SKDDriverProperty()
			{
				Name = "Login",
				Caption = "Логин",
				DriverPropertyType = GKDriverPropertyTypeEnum.StringType,
				StringDefault = ""
			};
			driver.Properties.Add(loginProperty);

			var passwordProperty = new SKDDriverProperty()
			{
				Name = "Password",
				Caption = "Пароль",
				DriverPropertyType = GKDriverPropertyTypeEnum.StringType,
				StringDefault = ""
			};
			driver.Properties.Add(passwordProperty);

			return driver;
		}
	}
}