using System;
using GKModule.Models;
using XFiresecAPI;

namespace GKModule.Converter
{
    public static class GKHelper
    {
        public static XDriver Create()
        {
            var xDriver = new XDriver()
            {
                DriverType = XDriverType.GK,
                UID = DriversHelper.GK_UID,
                OldDriverUID = Guid.Empty,
                CanEditAddress = false,
                HasAddress = false,
                ImageSource = FiresecClient.FileHelper.GetIconFilePath("Device_Device.ico"),
                HasImage = true,
                IsChildAddressReservedRange = false,
                Name = "Групповой контроллер",
                ShortName = "ГК"
            };
			xDriver.AutoCreateChildren.Add(DriversHelper.GKIndicator_UID);
            xDriver.Children.Add(DriversHelper.KAU_UID);

            xDriver.Properties.Add(
                new XDriverProperty()
                {
                    Name = "MacAddress1",
                    Caption = "Мак адрес 1",
                    ToolTip = "Мак адрес 1",
                    Default = "1, 2, 3, 4, 5, 6",
                    DriverPropertyType = XDriverPropertyTypeEnum.StringType,
                    IsInternalDeviceParameter = false
                }
                );

            xDriver.Properties.Add(
                new XDriverProperty()
                {
                    Name = "MacAddress2",
                    Caption = "Мак адрес 2",
                    ToolTip = "Мак адрес 2",
                    Default = "2, 2, 2, 2, 2, 2",
                    DriverPropertyType = XDriverPropertyTypeEnum.StringType,
                    IsInternalDeviceParameter = false
                }
                );

            xDriver.Properties.Add(
                new XDriverProperty()
                {
                    Name = "IPAddress1",
                    Caption = "IP адрес 1",
                    ToolTip = "IP адрес 1",
                    Default = "126.1.1.2",
                    DriverPropertyType = XDriverPropertyTypeEnum.StringType,
                    IsInternalDeviceParameter = false
                }
                );

            xDriver.Properties.Add(
                new XDriverProperty()
                {
                    Name = "IPAddress2",
                    Caption = "IP адрес 2",
                    ToolTip = "IP адрес 2",
                    Default = "126.1.1.1",
                    DriverPropertyType = XDriverPropertyTypeEnum.StringType,
                    IsInternalDeviceParameter = false
                }
                );

            return xDriver;
        }
    }
}