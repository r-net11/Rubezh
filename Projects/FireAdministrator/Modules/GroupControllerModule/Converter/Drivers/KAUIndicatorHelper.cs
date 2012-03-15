using System;
using GroupControllerModule.Models;

namespace GroupControllerModule.Converter
{
    public class KAUIndicatorHelper
    {
        public static XDriver Create()
        {
            var xDriver = new XDriver()
            {
                DriverType = XDriverType.KAUIndicator,
                UID = DriversHelper.KAUIndicator_UID,
                OldDriverUID = Guid.Empty,
                CanEditAddress = false,
                HasAddress = false,
                IsAutoCreate = true,
                MinAutoCreateAddress = 1,
                MaxAutoCreateAddress = 1,
                ImageSource = FiresecClient.FileHelper.GetIconFilePath("Device_Device.ico"),
                HasImage = true,
                IsChildAddressReservedRange = false,
                Name = "Индикатор",
                ShortName = "Индикатор"
            };

            var modeProperty = new XDriverProperty()
            {
                No = 0,
                Name = "Mode",
                Caption = "Режим работы",
                ToolTip = "Режим работы индикатора",
                Default = 0,
                DriverPropertyType = XDriverPropertyTypeEnum.EnumType,
                IsInternalDeviceParameter = true
            };
            modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Не гореть", Value = 0 });
            modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Гореть", Value = 1 });
            modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Мигать", Value = 2 });
            xDriver.Properties.Add(modeProperty);

            xDriver.Properties.Add(
                new XDriverProperty()
                {
                    No = 1,
                    Name = "OnDuration",
                    Caption = "Продолжительность горения для режима 2",
                    ToolTip = "Продолжительность горения для режима 2",
                    Default = 0,
                    DriverPropertyType = XDriverPropertyTypeEnum.IntType,
                    IsInternalDeviceParameter = true
                }
                );

            xDriver.Properties.Add(
                new XDriverProperty()
                {
                    No = 2,
                    Name = "OnDuration",
                    Caption = "Продолжительность гашения для режима 2",
                    ToolTip = "Продолжительность гашения для режима 2",
                    DriverPropertyType = XDriverPropertyTypeEnum.IntType,
                    Default = 0,
                    IsInternalDeviceParameter = true
                }
                );

            return xDriver;
        }
    }
}