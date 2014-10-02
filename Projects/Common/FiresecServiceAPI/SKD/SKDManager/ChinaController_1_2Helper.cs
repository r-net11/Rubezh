using System;
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
				DefaultDoorType = DoorType.TwoWay,
				CanChangeDoorType = false,
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