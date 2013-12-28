using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace GKProcessor
{
    public static class JockeyPump_Helper
    {
        public static XDriver Create()
        {
            var driver = new XDriver()
            {
                DriverTypeNo = 0x70,
                DriverType = XDriverType.JockeyPump,
                UID = new Guid("1EB96235-8275-445F-9EB2-8F92157764F9"),
                Name = "Шкаф управления жокей насосом",
                ShortName = "ШУН-ЖН",
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

            GKDriversHelper.AddIntProprety(driver, 0x84, "Время ожидания восстановления давления, мин", 2, 2, 65535);

            var property3 = new XDriverProperty()
            {
                No = 0x8d,
                Name = "Датчик низкого давления",
                Caption = "Датчик низкого давления",
                DriverPropertyType = XDriverPropertyTypeEnum.BoolType,
                IsLowByte = true,
                Mask = 1
            };
            driver.Properties.Add(property3);

            var property4 = new XDriverProperty()
            {
                No = 0x8d,
                Name = "Датчик высокого давления",
                Caption = "Датчик высокого давления",
                DriverPropertyType = XDriverPropertyTypeEnum.BoolType,
                IsLowByte = true,
                Mask = 2
            };
            driver.Properties.Add(property4);

            var manometerProperty = new XDriverProperty()
            {
                No = 0x8d,
                Name = "Манометр",
                Caption = "Манометр",
                ToolTip = "Тип манометра",
                Default = 0,
                DriverPropertyType = XDriverPropertyTypeEnum.EnumType,
                IsHieghByte = true,
                Mask = 1
            };
            manometerProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Одноконтактный", Value = 0 });
            manometerProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Двухконтактный", Value = 1 });
            driver.Properties.Add(manometerProperty);

            driver.MeasureParameters.Add(new XMeasureParameter() { No = 0x80, Name = "Режим работы" });
            return driver;
        }
    }
}
