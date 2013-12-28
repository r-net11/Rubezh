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
                Name = "Датчик высокого уровня",
                Caption = "Датчик высокого уровня",
                DriverPropertyType = XDriverPropertyTypeEnum.BoolType,
                IsLowByte = true,
                Mask = 1
            };
            driver.Properties.Add(property3);

            var property4 = new XDriverProperty()
            {
                No = 0x8d,
                Name = "Датчик низкого уровня",
                Caption = "Датчик низкого уровня",
                DriverPropertyType = XDriverPropertyTypeEnum.BoolType,
                IsLowByte = true,
                Mask = 2
            };
            driver.Properties.Add(property4);

            var property5 = new XDriverProperty()
            {
                No = 0x8d,
                Name = "Датчик аварийного уровня",
                Caption = "Датчик аварийного уровня",
                DriverPropertyType = XDriverPropertyTypeEnum.BoolType,
                IsLowByte = true,
                Mask = 4
            };
            driver.Properties.Add(property5);

            driver.MeasureParameters.Add(new XMeasureParameter() { No = 0x80, Name = "Режим работы" });
            return driver;
        }
    }
}
