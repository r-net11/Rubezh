using System.Linq;
using FiresecClient;
using XFiresecAPI;

namespace Common.GK
{
	public static class AM1Helper
	{
		public static void Create()
		{
			var xDriver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.AM_1);

			var property1 = new XDriverProperty()
			{
				No = 0x81,
				Name = "Конфигурация",
				Caption = "Конфигурация",
				Default = 0
			};
			var property1Parameter1 = new XDriverPropertyParameter()
			{
				Name = "1 1 Замкнуто",
				Value = 1
			};
			var property1Parameter2 = new XDriverPropertyParameter()
			{
				Name = "2 1 Разомкнуто",
				Value = 2
			};
			var property1Parameter3 = new XDriverPropertyParameter()
			{
				Name = "3 2 Замкнуто",
				Value = 3
			};
			var property1Parameter4 = new XDriverPropertyParameter()
			{
				Name = "3 4 Разомкнуто",
				Value = 4
			};
			property1.Parameters.Add(property1Parameter1);
			property1.Parameters.Add(property1Parameter2);
			property1.Parameters.Add(property1Parameter3);
			property1.Parameters.Add(property1Parameter4);
			xDriver.Properties.Add(property1);
		}
	}
}