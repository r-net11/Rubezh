using System;
using XFiresecAPI;

namespace Common.GK
{
	public class GKRele_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x105,
				DriverType = XDriverType.GKRele,
				UID = new Guid("1AC85436-61BC-441B-B6BF-C6A0FA62748B"),
				Name = "Реле ГК",
				ShortName = "Реле ГК",
				HasLogic = true,
				CanEditAddress = false,
                HasAddress = false,
				IsAutoCreate = true,
				MinAddress = 14,
				MaxAddress = 16,
				IsDeviceOnShleif = false,
				IsPlaceable = true
			};

			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);
			driver.AvailableStateClasses.Add(XStateClass.On);

			var modeProperty = new XDriverProperty()
			{
				No = 0,
				Name = "Mode",
				Caption = "Режим работы",
				ToolTip = "Режим работы реле",
				Default = 15,
				DriverPropertyType = XDriverPropertyTypeEnum.EnumType,
				IsAUParameter = true
			};
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Выключено", Value = 0 });
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Мерцает 0.25 с", Value = 1 });
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Мерцает 0.5 с", Value = 3 });
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Мерцает 0.75 с", Value = 7 });
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Включено", Value = 15 });
			driver.Properties.Add(modeProperty);

			return driver;
		}
	}
}