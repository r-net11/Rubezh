using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace GKProcessor
{
    public static class FirePump_Helper
    {
        public static XDriver Create()
        {
            var driver = new XDriver()
            {
                DriverTypeNo = 0x70,
                DriverType = XDriverType.FirePump,
                UID = new Guid("CE578ED6-F39B-4A92-9F03-BB92A904A14C"),
                Name = "Шкаф управления пожарным насосом",
                ShortName = "ШУН-ПН",
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

            GKDriversHelper.AddIntProprety(driver, 0x84, "Время ожидания выхода насоса на режим, c", 3, 3, 30);

            var property3 = new XDriverProperty()
            {
                No = 0x8d,
                Name = "ЭКМ на выходе насоса НЗ",
                Caption = "ЭКМ на выходе насоса НЗ",
                DriverPropertyType = XDriverPropertyTypeEnum.BoolType,
                IsLowByte = true,
                Mask = 1
            };
            driver.Properties.Add(property3);

            var property4 = new XDriverProperty()
            {
                No = 0x8d,
                Name = "Кнопка дистанционного управления Старт",
                Caption = "Кнопка дистанционного управления Старт",
                DriverPropertyType = XDriverPropertyTypeEnum.BoolType,
                IsLowByte = true,
                Mask = 2
            };
            driver.Properties.Add(property4);

            var property5 = new XDriverProperty()
            {
                No = 0x8d,
                Name = "Кнопка дистанционного управления Стоп",
                Caption = "Кнопка дистанционного управления Стоп",
                DriverPropertyType = XDriverPropertyTypeEnum.BoolType,
                IsLowByte = true,
                Mask = 4
            };
            driver.Properties.Add(property5);

            var property6 = new XDriverProperty()
            {
                No = 0x8d,
                Name = "Дистанционное управлени Вкл",
                Caption = "Дистанционное управлени Вкл",
                DriverPropertyType = XDriverPropertyTypeEnum.BoolType,
                IsLowByte = true,
                Mask = 8
            };
            driver.Properties.Add(property6);

            driver.MeasureParameters.Add(new XMeasureParameter() { No = 0x80, Name = "Режим работы" });
            return driver;
        }
    }
}
