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
                DriverType = XDriverType.KAU,
                UID = DriversHelper.KAU_UID,
                OldDriverUID = Guid.Empty,
                CanEditAddress = true,
                HasAddress = true,
                ImageSource = FiresecClient.FileHelper.GetIconFilePath("Device_Device.ico"),
                HasImage = true,
                IsChildAddressReservedRange = false,
                Name = "Контроллер адресных устройств",
                ShortName = "КАУ"
            };
            xDriver.AutoCreateChildren.Add(DriversHelper.KAUIndicator_UID);
            xDriver.AutoCreateChildren.Add(DriversHelper.KAUExit_UID);
            foreach (var driver in XManager.DriversConfiguration.Drivers)
            {
                if (DriversHelper.Drivers.Any(x=>x.ConnectToAddressController))
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
                    ToolTip = "Порог питания основного",
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
                    ToolTip = "Порог питания резервного",
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
                    ToolTip = "Число неответов адресных устройств",
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
                    ToolTip = "Интервал опроса шлейфов",
                    Default = 1,
                    DriverPropertyType = XDriverPropertyTypeEnum.IntType,
                    IsInternalDeviceParameter = true
                }
                );


            return xDriver;
        }
    }
}