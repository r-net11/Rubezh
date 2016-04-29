using System;
using System.Collections.Generic;
using RubezhAPI.GK;

namespace GKProcessor
{
	public static class RSR2_MVP_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverTypeNo = 0xE5,
				DriverType = GKDriverType.RSR2_MVP,
				UID = new Guid("0B1BD00D-680B-4A80-AC9A-659FD7F85BB4"),
				Name = "Модель ветвления и подпитки",
				ShortName = "МВП",
				IsPlaceable = true,
				AutoCreateChildren = new List<GKDriverType>() { GKDriverType.RSR2_MVP_Part },
				DriverClassification = GKDriver.DriverClassifications.Other
			};

			GKDriversHelper.AddAvailableStateBits(driver, GKStateBit.Test);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Test);

			GKDriversHelper.AddIntProprety(driver, 0, "Число АУ на АЛС3 МВП", 0, 0, 250).CanNotEdit = true;
			GKDriversHelper.AddIntProprety(driver, 1, "Число АУ на АЛС4 МВП", 0, 0, 250).CanNotEdit = true;
			var property = new GKDriverProperty()
			{
				No = 2,
				Name = "Порог КЗ, В",
				Caption = "Порог КЗ, В",
				DriverPropertyType = GKDriverPropertyTypeEnum.IntType,
				Default = (ushort)330,
				Min = (ushort)300,
				Max = (ushort)700,
				Multiplier = 10
			};
			driver.Properties.Add(property);

			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 1, Name = "Напряжение на АЛC1, В", InternalName = "Voltage_1", Multiplier = 10 });
			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 2, Name = "Напряжение на АЛС2, В", InternalName = "Voltage_2", Multiplier = 10 });
			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 3, Name = "Напряжение на АЛС3, В", InternalName = "Voltage_3", Multiplier = 10 });
			driver.MeasureParameters.Add(new GKMeasureParameter() { No = 4, Name = "Напряжение на АЛС4, В", InternalName = "Voltage_4", Multiplier = 10 });

			return driver;
		}
	}
}