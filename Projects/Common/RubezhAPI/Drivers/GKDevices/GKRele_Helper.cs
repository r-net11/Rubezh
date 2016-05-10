using System;
using RubezhAPI.GK;

namespace RubezhAPI
{
	public class GKRele_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0x105,
				DriverType = GKDriverType.GKRele,
				UID = new Guid("1AC85436-61BC-441B-B6BF-C6A0FA62748B"),
				Name = "Реле ГК",
				ShortName = "Реле ГК",
				IsControlDevice = true,
				HasLogic = true,
				HasAddress = false,
				IsAutoCreate = true,
				MinAddress = 12,
				MaxAddress = 16,
				IsDeviceOnShleif = false,
				IsPlaceable = true
			};

			driver.AvailableStateBits.Add(GKStateBit.On);
			driver.AvailableStateBits.Add(GKStateBit.Off);

			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);
			driver.AvailableStateClasses.Add(XStateClass.On);
			driver.AvailableStateClasses.Add(XStateClass.Ignore);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(GKStateBit.TurnOff_InManual);

			var modeProperty = new GKDriverProperty()
			{
				No = 0,
				Name = "Mode",
				Caption = "Режим работы",
				ToolTip = "Режим работы реле",
				Default = 15,
				DriverPropertyType = GKDriverPropertyTypeEnum.EnumType,
				IsAUParameter = true
			};
			modeProperty.Parameters.Add(new GKDriverPropertyParameter() { Name = "Выключено", Value = 0 });
			modeProperty.Parameters.Add(new GKDriverPropertyParameter() { Name = "Мерцает 0.25 с", Value = 1 });
			modeProperty.Parameters.Add(new GKDriverPropertyParameter() { Name = "Мерцает 0.5 с", Value = 3 });
			modeProperty.Parameters.Add(new GKDriverPropertyParameter() { Name = "Мерцает 0.75 с", Value = 7 });
			modeProperty.Parameters.Add(new GKDriverPropertyParameter() { Name = "Включено", Value = 15 });
			driver.Properties.Add(modeProperty);

			return driver;
		}
	}
}