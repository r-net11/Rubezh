using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace Common.GK
{
	public static class AM1_O_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x51,
				DriverType = XDriverType.AM1_O,
				UID = new Guid("efca74b2-ad85-4c30-8de8-8115cc6dfdd2"),
				Name = "Охранная адресная метка АМ1-О",
				ShortName = "АМ1-О",
				HasZone = true
			};

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
			driver.Properties.Add(property1);

			return driver;
		}
	}
}