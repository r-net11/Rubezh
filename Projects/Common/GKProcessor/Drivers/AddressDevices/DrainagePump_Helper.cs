using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace GKProcessor
{
    public static class DrainagePump_Helper
    {
        public static XDriver Create()
        {
            var driver = new XDriver()
            {
                DriverTypeNo = 0x70,
                DriverType = XDriverType.DrainagePump,
                UID = new Guid("FF1245BF-C923-4751-9A75-BDFC18CA0996"),
                Name = "Шкаф управления дренажным насосом",
                ShortName = "ШУН-ДН",
                IsControlDevice = true,
                HasLogic = false,
                IsPlaceable = true,
                MaxAddressOnShleif = 15
            };

            GKDriversHelper.AddControlAvailableStates(driver);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOn);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOff);
            GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Off);


            driver.AvailableCommandBits.Add(XStateBit.TurnOn_InManual);
            driver.AvailableCommandBits.Add(XStateBit.TurnOnNow_InManual);
            driver.AvailableCommandBits.Add(XStateBit.TurnOff_InManual);

            var property3 = new XDriverProperty()
            {
                No = 0x8d,
                Name = "Тип контакта датчика ВУ",
				Caption = "Тип контакта датчика ВУ",
                DriverPropertyType = XDriverPropertyTypeEnum.EnumType,
                IsLowByte = true,
                Mask = 1
            };
			property3.Parameters.Add(new XDriverPropertyParameter() { Name = "Нормально разомкнутый", Value = 0 });
			property3.Parameters.Add(new XDriverPropertyParameter() { Name = "Нормально замкнутый", Value = 1 });
			driver.Properties.Add(property3);

            var property4 = new XDriverProperty()
            {
                No = 0x8d,
				Name = "Тип контакта датчика НУ",
				Caption = "Тип контакта датчика НУ",
				DriverPropertyType = XDriverPropertyTypeEnum.EnumType,
                IsLowByte = true,
                Mask = 2
            };
			property4.Parameters.Add(new XDriverPropertyParameter() { Name = "Нормально разомкнутый", Value = 0 });
			property4.Parameters.Add(new XDriverPropertyParameter() { Name = "Нормально замкнутый", Value = 1 });
            driver.Properties.Add(property4);

            var property5 = new XDriverProperty()
            {
                No = 0x8d,
				Name = "Тип контакта датчика АУ",
				Caption = "Тип контакта датчика АУ",
				DriverPropertyType = XDriverPropertyTypeEnum.EnumType,
                IsLowByte = true,
                Mask = 4
            };
			property5.Parameters.Add(new XDriverPropertyParameter() { Name = "Нормально разомкнутый", Value = 0 });
			property5.Parameters.Add(new XDriverPropertyParameter() { Name = "Нормально замкнутый", Value = 1 });
            driver.Properties.Add(property5);

            driver.MeasureParameters.Add(new XMeasureParameter() { No = 0x80, Name = "Режим работы" });
            return driver;
        }
    }
}
