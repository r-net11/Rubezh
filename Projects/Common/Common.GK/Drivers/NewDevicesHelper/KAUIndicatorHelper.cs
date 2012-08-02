using System;
using XFiresecAPI;
using FiresecClient;

namespace Common.GK
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
				ImageSource = FileHelper.GetIconFilePath("Device_Device") + ".png",
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
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Выключено", Value = 0 });
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Включено", Value = 1 });
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Мерцает", Value = 2 });
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
                    Name = "OffDuration",
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