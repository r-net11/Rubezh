using System.Collections.Generic;
using System.Linq;

namespace FiresecAPI.Models
{
	public class MROHelper
	{
		public static void Create(List<Driver> drivers)
		{
			var driver = drivers.FirstOrDefault(x => x.DriverType == DriverType.MRO);
			driver.HasConfigurationProperties = true;

			var property1 = new DriverProperty()
			{
				IsInternalDeviceParameter = true,
				No = 0x82,
				Name = "Количество повторов",
				Caption = "Количество повторов",
				Default = "0",
				DriverPropertyType = DriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 255
			};
			driver.Properties.Add(property1);

			var property2 = new DriverProperty()
			{
				IsInternalDeviceParameter = true,
				No = 0x88,
				Name = "Время отложенного пуска",
				Caption = "Время отложенного пуска",
				Default = "0",
				DriverPropertyType = DriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 255
			};
			driver.Properties.Add(property2);
		}
	}
}