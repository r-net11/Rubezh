using System;
using GroupControllerModule.Models;
using XFiresecAPI;

namespace GroupControllerModule.Converter
{
    public static class KAUExitHelper
    {
        public static XDriver Create()
        {
            var xDriver = new XDriver()
            {
                DriverType = XDriverType.KAUExit,
                UID = DriversHelper.KAUExit_UID,
                OldDriverUID = Guid.Empty,
                CanEditAddress = false,
                HasAddress = true,
                IsAutoCreate = true,
                MinAutoCreateAddress = 1,
                MaxAutoCreateAddress = 4,
                ImageSource = FiresecClient.FileHelper.GetIconFilePath("Device_Device.ico"),
                HasImage = true,
                IsChildAddressReservedRange = false,
                Name = "Выход",
                ShortName = "Выход"
            };
            return xDriver;
        }
    }
}