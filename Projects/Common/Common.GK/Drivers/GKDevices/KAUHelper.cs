using System;
using System.Linq;
using FiresecClient;
using XFiresecAPI;

namespace Common.GK
{
    public static class KAUHelper
    {
        public static XDriver Create()
        {
            var driver = new XDriver()
            {
                DriverTypeNo = 0x102,
                DriverType = XDriverType.KAU,
				UID = new Guid("4993E06C-85D1-4F20-9887-4C5F67C450E8"),
				Name = "Контроллер адресных устройств",
				ShortName = "КАУ",
                CanEditAddress = true,
                HasAddress = true,
				IsDeviceOnShleif = false,
                IsChildAddressReservedRange = false,
                IsRangeEnabled = true,
                MinAddress = 1,
                MaxAddress = 127
            };
            driver.AutoCreateChildren.Add(XDriverType.KAUIndicator);

            driver.Properties.Add(
                new XDriverProperty()
                {
                    No = 0,
                    Name = "Parameter 0",
                    Caption = "Порог питания основного",
                    ToolTip = "",
                    Default = 200,
                    DriverPropertyType = XDriverPropertyTypeEnum.IntType,
                    IsInternalDeviceParameter = true
                }
                );

            driver.Properties.Add(
                new XDriverProperty()
                {
                    No = 1,
                    Name = "Parameter 1",
                    Caption = "Порог питания резервного",
                    ToolTip = "",
                    Default = 200,
                    DriverPropertyType = XDriverPropertyTypeEnum.IntType,
                    IsInternalDeviceParameter = true
                }
                );

            driver.Properties.Add(
                new XDriverProperty()
                {
                    No = 2,
                    Name = "Parameter 2",
                    Caption = "Число неответов адресных устройств",
                    ToolTip = "",
                    Default = 5,
                    DriverPropertyType = XDriverPropertyTypeEnum.IntType,
                    IsInternalDeviceParameter = true
                }
                );

            driver.Properties.Add(
                new XDriverProperty()
                {
                    No = 3,
                    Name = "Parameter 3",
                    Caption = "Интервал опроса шлейфов",
                    ToolTip = "",
                    Default = 1000,
                    DriverPropertyType = XDriverPropertyTypeEnum.IntType,
                    IsInternalDeviceParameter = true
                }
                );

            var modeProperty = new XDriverProperty()
            {
                Name = "Mode",
                Caption = "Линия",
                ToolTip = "",
                Default = 0,
                DriverPropertyType = XDriverPropertyTypeEnum.EnumType,
                IsInternalDeviceParameter = false
            };
            modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Основная", Value = 0 });
            modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Резервная", Value = 1 });
            driver.Properties.Add(modeProperty);

            return driver;
        }
    }
}