using System;
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
				DefaultDoorType = DoorType.OneWay,
				CanChangeDoorType = true,
				IsPlaceable = true
			};
			driver.Children.Add(SKDDriverType.Reader);
			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Failure);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);

			driver.AutocreationItems.Add(new SKDDriverAutocreationItem(SKDDriverType.Reader, 2));
			driver.AutocreationItems.Add(new SKDDriverAutocreationItem(SKDDriverType.Lock, 2));
			driver.AutocreationItems.Add(new SKDDriverAutocreationItem(SKDDriverType.LockControl, 2));
			driver.AutocreationItems.Add(new SKDDriverAutocreationItem(SKDDriverType.Button, 2));

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
				Default = 37777
			};
			driver.Properties.Add(driverProperty);

			var loginProperty = new SKDDriverProperty()
			{
				Name = "Login",
				Caption = "Логин",
				DriverPropertyType = GKDriverPropertyTypeEnum.StringType,
				StringDefault = "admin"
			};
			driver.Properties.Add(loginProperty);

			var passwordProperty = new SKDDriverProperty()
			{
				Name = "Password",
				Caption = "Пароль",
				DriverPropertyType = GKDriverPropertyTypeEnum.StringType,
				StringDefault = "123456"
			};
			driver.Properties.Add(passwordProperty);

			return driver;
		}
	}
}