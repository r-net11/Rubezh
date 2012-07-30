using System;
using XFiresecAPI;

namespace Commom.GK
{
    public class KAUIndicatorHelper
    {
        public static XDriver Create()
        {
            var xDriver = new XDriver()
            {
                DriverTypeNo = 0x103,
                DriverType = XDriverType.KAUIndicator,
                UID = GKDriversHelper.KAUIndicator_UID,
                OldDriverUID = Guid.Empty,
                CanEditAddress = false,
                HasAddress = false,
                IsAutoCreate = true,
                MinAutoCreateAddress = 1,
                MaxAutoCreateAddress = 1,
                ImageSource = FiresecClient.FileHelper.GetIconFilePath("Device_Device.ico"),
                HasImage = true,
                IsChildAddressReservedRange = false,
                Name = "Индикатор КАУ",
				ShortName = "Индикатор КАУ"
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
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Гореть 0.25 сек", Value = 1 });
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Гореть 0.5 сек", Value = 3 });
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Гореть 0.75 сек", Value = 7 });
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Мигать", Value = 15 });
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