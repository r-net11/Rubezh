using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace Common.GK
{
	public static class AM1_T_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x51,
				DriverType = XDriverType.AM1_T,
				UID = new Guid("f5a34ce2-322e-4ed9-a75f-fc8660ae33d8"),
				Name = "Технологическая адресная метка АМ1-Т",
				ShortName = "АМ1-Т",
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