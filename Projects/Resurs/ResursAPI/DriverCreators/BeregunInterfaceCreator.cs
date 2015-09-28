using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public class BeregunInterfaceCreator
	{
		public static Driver Create()
		{
			var driver = new Driver();
			driver.DriverType = DriverType.BeregunInterface;
			driver.AddParameter("Скорость интерфейса", ParameterType.Enum, parameterEnumItems: new List<ParameterEnumItem>
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
				});
			driver.AddParameter("Таймаут ответа, сек", ParameterType.Enum, parameterEnumItems: new List<ParameterEnumItem>
				{
					new ParameterEnumItem{ Name = "1", Value = 0 },
					new ParameterEnumItem{ Name = "2", Value = 1 },
					new ParameterEnumItem{ Name = "3", Value = 2 },
					new ParameterEnumItem{ Name = "5", Value = 3 },
					new ParameterEnumItem{ Name = "7", Value = 4 },
					new ParameterEnumItem{ Name = "10", Value = 5 },
				});
			driver.Children.Add(DriverType.BeregunCounter);
			return driver;
		}
	}
}
