using System.Linq;
using FiresecClient;
using XFiresecAPI;

namespace Commom.GK
{
	public class MROHelper
	{
		public static void Create()
		{
			var xDriver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.MRO);

			var property1 = new XDriverProperty()
			{
				No = 0x82,
				Name = "Количество повторов",
				Caption = "Количество повторов",
				Default = 0,
				DriverPropertyType = XDriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 255
			};
			xDriver.Properties.Add(property1);

			var property2 = new XDriverProperty()
			{
				No = 0x88,
				Name = "Время отложенного пуска",
				Caption = "Время отложенного пуска",
				Default = 0,
				DriverPropertyType = XDriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 255
			};
			xDriver.Properties.Add(property2);
		}
	}
}