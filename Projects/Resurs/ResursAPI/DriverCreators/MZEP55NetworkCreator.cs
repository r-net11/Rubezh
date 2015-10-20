using ResursAPI.ParameterNames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public static class MZEP55NetworkCreator
	{
		public static Driver Create()
		{
			var driver = new Driver(new Guid("8219D556-D828-4DD1-9CAC-4EEF0E3A8C8F"));
			driver.DriverType = DriverType.MZEP55Network;
			driver.DeviceType = DeviceType.Network;
			driver.Children.Add(DriverType.MZEP55Counter);
			driver.DriverParameters.Add(new DriverParameter(new Guid("5DD69238-95D0-4757-B8C9-BBB025363DC7"))
			{
				Name = ParameterNamesMZEP55Network.BautRate,
				Description = "Скорость интерфейса",
				ParameterType = ParameterType.Enum,
				ParameterEnumItems = new List<ParameterEnumItem>
				{
					new ParameterEnumItem{ Name = "2400", Value = 0 },
					new ParameterEnumItem{ Name = "4800", Value = 1 },
					new ParameterEnumItem{ Name = "9600", Value = 2 },
					new ParameterEnumItem{ Name = "19200", Value = 3 },
				},
				EnumDefaultItem = 2,
				Number = 0
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("29054120-0501-487A-95EC-FEEA8F583DFD"))
			{
				Name = ParameterNamesMZEP55Network.Timeout,
				Description = "Таймаут ответа, мс",
				ParameterType = ParameterType.Int,
				IntMinValue = 0,
				IntDefaultValue = 1,
				Number = 1
			});
			return driver;
		}
	}
}
