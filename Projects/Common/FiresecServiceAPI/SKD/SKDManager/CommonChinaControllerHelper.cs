using System.Collections.Generic;

namespace StrazhAPI.SKD
{
	public static class CommonChinaControllerHelper
	{
		public static List<SKDDriverProperty> CreateProperties()
		{
			var properties = new List<SKDDriverProperty>();

			var addressProperty = new SKDDriverProperty()
			{
				Name = "Address",
				Caption = "Адрес",
				DriverPropertyType = SKDDriverType.StringType,
				StringDefault = "192.168.0.2"
			};
			properties.Add(addressProperty);

			var portProperty = new SKDDriverProperty()
			{
				Name = "Port",
				Caption = "Порт",
				DriverPropertyType = SKDDriverType.IntType,
				Default = 37777
			};
			properties.Add(portProperty);

			var loginProperty = new SKDDriverProperty()
			{
				Name = "Login",
				Caption = "Логин",
				DriverPropertyType = SKDDriverType.StringType,
				StringDefault = "admin"
			};
			properties.Add(loginProperty);

			var passwordProperty = new SKDDriverProperty()
			{
				Name = "Password",
				Caption = "Пароль",
				DriverPropertyType = SKDDriverType.StringType,
				StringDefault = "123456"
			};
			properties.Add(passwordProperty);

			var maskProperty = new SKDDriverProperty()
			{
				Name = "Mask",
				Caption = "Маска подсети",
				DriverPropertyType = SKDDriverType.StringType,
				StringDefault = "255.255.255.0"
			};
			properties.Add(maskProperty);

			var gatewayProperty = new SKDDriverProperty()
			{
				Name = "Gateway",
				Caption = "Шлюз по умолчанию",
				DriverPropertyType = SKDDriverType.StringType,
				StringDefault = "192.168.0.1"
			};
			properties.Add(gatewayProperty);

			return properties;
		}
	}
}