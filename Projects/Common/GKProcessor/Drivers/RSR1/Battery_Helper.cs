using System;
using FiresecAPI.GK;

namespace GKProcessor
{
	public static class Battery_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0xD6,
				DriverType = GKDriverType.Battery,
				UID = new Guid("0D24D46E-11E5-41E2-B0E8-19002F2AB295"),
				Name = "Адресный источник питания ИВЭПР-RSR",
				ShortName = "ИВЭПР",
				IsPlaceable = true,
				IsIgnored = true,
			};

			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);

			var property1 = new GKDriverProperty()
			{
				No = 0x84,
				Name = "Конфигурация",
				Caption = "Конфигурация",
				Default = 1,
			};
			var property1Parameter1 = new GKDriverPropertyParameter()
			{
				Name = "1 АКБ",
				Value = 1
			};
			var property1Parameter2 = new GKDriverPropertyParameter()
			{
				Name = "2 АКБ",
				Value = 2
			};
			property1.Parameters.Add(property1Parameter1);
			property1.Parameters.Add(property1Parameter2);
			driver.Properties.Add(property1);

			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 0x81, Name = "Напряжение на АКБ1", InternalName = "InnerVoltage_1", IsHighByte = true, Multiplier = 10 });
			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 0x81, Name = "Напряжение на АКБ2", InternalName = "InnerVoltage_2", IsLowByte = true, Multiplier = 10 });
			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 0x82, Name = "Напряжение на выходе 1", InternalName = "OuterVoltage_1", IsHighByte = true, Multiplier = 10 });
			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 0x82, Name = "Напряжение на выходе 2", InternalName = "OuterVoltage_2", IsLowByte = true, Multiplier = 10 });
			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 0x83, Name = "Напряжение в сети", InternalName = "CircuitVoltage", IsLowByte = true });
			
			return driver;
		}
	}
}