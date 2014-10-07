using System;
using FiresecAPI.GK;
using System.Collections.Generic;

namespace FiresecAPI.SKD
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
				DriverPropertyType = GKDriverPropertyTypeEnum.StringType,
				StringDefault = "192.168.0.1"
			};
			properties.Add(addressProperty);

			var portProperty = new SKDDriverProperty()
			{
				Name = "Port",
				Caption = "Порт",
				DriverPropertyType = GKDriverPropertyTypeEnum.IntType,
				Default = 37777
			};
			properties.Add(portProperty);

			var loginProperty = new SKDDriverProperty()
			{
				Name = "Login",
				Caption = "Логин",
				DriverPropertyType = GKDriverPropertyTypeEnum.StringType,
				StringDefault = "admin"
			};
			properties.Add(loginProperty);

			var passwordProperty = new SKDDriverProperty()
			{
				Name = "Password",
				Caption = "Пароль",
				DriverPropertyType = GKDriverPropertyTypeEnum.StringType,
				StringDefault = "123456"
			};
			properties.Add(passwordProperty);

			var maskProperty = new SKDDriverProperty()
			{
				Name = "Mask",
				Caption = "Маска подсети",
				DriverPropertyType = GKDriverPropertyTypeEnum.StringType,
				StringDefault = "255.255.255.255"
			};
			properties.Add(maskProperty);

			var gatewayProperty = new SKDDriverProperty()
			{
				Name = "Gateway",
				Caption = "Шлюз по умолчанию",
				DriverPropertyType = GKDriverPropertyTypeEnum.StringType,
				StringDefault = "192.168.0.1"
			};
			properties.Add(gatewayProperty);

			return properties;
		}
	}
}