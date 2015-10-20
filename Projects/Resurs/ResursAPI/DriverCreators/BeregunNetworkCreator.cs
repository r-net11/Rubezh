using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public class BeregunNetworkCreator
	{
		public static Driver Create()
		{
			var driver = new Driver(new Guid("B86CEDA6-1560-4892-8F9A-586C346026AF"));
			driver.DriverType = DriverType.BeregunNetwork;
			driver.DeviceType = DeviceType.Network;
			driver.Children.Add(DriverType.BeregunCounter);
			driver.DriverParameters.Add(new DriverParameter(new Guid("0DE0B86E-D1B9-4569-9998-5DC7DC079AE2"))
			{
				Name = "Скорость интерфейса",
				Description = "Скорость интерфейса",
				ParameterType = ParameterType.Enum,
				ParameterEnumItems = new List<ParameterEnumItem>
				{
					new ParameterEnumItem{ Name = "300", Value = 0 },
					new ParameterEnumItem{ Name = "600", Value = 1 },
					new ParameterEnumItem{ Name = "1200", Value = 2 },
					new ParameterEnumItem{ Name = "2400", Value = 3 },
					new ParameterEnumItem{ Name = "9600", Value = 4 },
					new ParameterEnumItem{ Name = "19200", Value = 5 },
					new ParameterEnumItem{ Name = "38400", Value = 6 },
					new ParameterEnumItem{ Name = "57600", Value = 7 },
					new ParameterEnumItem{ Name = "115200", Value = 8 },
				},
				EnumDefaultItem = 2,
				Number = 0,
				CanWriteInActive = false
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("D356BF86-24F8-4A72-A0FE-43A34ADB5425"))
			{
				Name = "Таймаут ответа, сек",
				Description = "Таймаут ответа, сек",
				ParameterType = ParameterType.Enum,
				ParameterEnumItems = new List<ParameterEnumItem>
				{
					new ParameterEnumItem{ Name = "1", Value = 0 },
					new ParameterEnumItem{ Name = "2", Value = 1 },
					new ParameterEnumItem{ Name = "3", Value = 2 },
					new ParameterEnumItem{ Name = "5", Value = 3 },
					new ParameterEnumItem{ Name = "7", Value = 4 },
					new ParameterEnumItem{ Name = "10", Value = 5 },
				},
				EnumDefaultItem = 5,
				Number = 1,
				CanWriteInActive = false
			});
			return driver;
		}
	}
}
