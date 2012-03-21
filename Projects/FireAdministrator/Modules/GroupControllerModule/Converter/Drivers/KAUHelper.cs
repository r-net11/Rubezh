using System;
using System.Linq;
using GroupControllerModule.Models;

namespace GroupControllerModule.Converter
{
    public static class KAUHelper
    {
        public static XDriver Create()
        {
            var xDriver = new XDriver()
            {
                DriverTypeNo = 0x102,
                DriverType = XDriverType.KAU,
                UID = DriversHelper.KAU_UID,
                OldDriverUID = Guid.Empty,
                CanEditAddress = true,
                HasAddress = true,
                ImageSource = FiresecClient.FileHelper.GetIconFilePath("Device_Device.ico"),
                HasImage = true,
                IsChildAddressReservedRange = false,
                Name = "Контроллер адресных устройств",
                ShortName = "КАУ",
                IsRangeEnabled = true,
                MinAddress = 1,
                MaxAddress = 127
            };
            xDriver.AutoCreateChildren.Add(DriversHelper.KAUIndicator_UID);
            //xDriver.AutoCreateChildren.Add(DriversHelper.KAUExit_UID);
            foreach (var driver in XManager.DriversConfiguration.Drivers)
            {
                var driverHelperItem = DriversHelper.Drivers.FirstOrDefault(x => x.XDriverType == driver.DriverType);
                if (driverHelperItem != null && driverHelperItem.ConnectToAddressController)
                {
                    xDriver.Children.Add(driver.UID);
                }
            }

            xDriver.Properties.Add(
                new XDriverProperty()
                {
                    No = 0,
                    Name = "Parameter 0",
                    Caption = "Порог питания основного",
                    ToolTip = "",
                    Default = 10,
                    DriverPropertyType = XDriverPropertyTypeEnum.IntType,
                    IsInternalDeviceParameter = true
                }
                );

            xDriver.Properties.Add(
                new XDriverProperty()
                {
                    No = 1,
                    Name = "Parameter 1",
                    Caption = "Порог питания резервного",
                    ToolTip = "",
                    Default = 10,
                    DriverPropertyType = XDriverPropertyTypeEnum.IntType,
                    IsInternalDeviceParameter = true
                }
                );

            xDriver.Properties.Add(
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

            xDriver.Properties.Add(
                new XDriverProperty()
                {
                    No = 3,
                    Name = "Parameter 3",
                    Caption = "Интервал опроса шлейфов",
                    ToolTip = "",
                    Default = 1,
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
            xDriver.Properties.Add(modeProperty);

            return xDriver;
        }
    }
}