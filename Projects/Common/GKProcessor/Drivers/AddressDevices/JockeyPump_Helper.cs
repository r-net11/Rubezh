using System;
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
                ShortName = "Жокей насос",
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

            GKDriversHelper.AddIntProprety(driver, 0x84, "Время ожидания ВД, мин", 2, 2, 65535);

            var property3 = new XDriverProperty()
            {
                No = 0x8d,
				Name = "Тип контакта датчика НД",
				Caption = "Тип контакта датчика НД",
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
				Name = "Тип контакта датчика ВД",
				Caption = "Тип контакта датчика ВД",
				DriverPropertyType = XDriverPropertyTypeEnum.EnumType,
                IsLowByte = true,
                Mask = 2
            };
			driver.Properties.Add(property4);
			property4.Parameters.Add(new XDriverPropertyParameter() { Name = "Нормально разомкнутый", Value = 0 });
			property4.Parameters.Add(new XDriverPropertyParameter() { Name = "Нормально замкнутый", Value = 2 });

            var manometerProperty = new XDriverProperty()
            {
                No = 0x8d,
                Name = "Конфигурация",
				Caption = "Конфигурация",
				ToolTip = "Тип конфигурации",
                Default = 0,
                DriverPropertyType = XDriverPropertyTypeEnum.EnumType,
                IsHieghByte = true,
                Mask = 1
            };
            manometerProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Два одноконтактных", Value = 0 });
            manometerProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Один двухконтактный", Value = 1 });
            driver.Properties.Add(manometerProperty);

            driver.MeasureParameters.Add(new XMeasureParameter() { No = 0x80, Name = "Режим работы" });
            return driver;
        }
    }
}
