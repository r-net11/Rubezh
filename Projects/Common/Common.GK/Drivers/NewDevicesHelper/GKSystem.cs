using System;
using XFiresecAPI;

namespace Commom.GK
{
    public static class GKSystemHelper
    {
        public static XDriver Create()
        {
            var xDriver = new XDriver()
            {
                DriverType = XDriverType.System,
                UID = GKDriversHelper.System_UID,
                OldDriverUID = Guid.Empty,
                CanEditAddress = false,
                HasAddress = false,
                ImageSource = FiresecClient.FileHelper.GetIconFilePath("Device_Device.ico"),
                HasImage = true,
                IsChildAddressReservedRange = false,
                Name = "Система",
                ShortName = "Система"
            };
            xDriver.Children.Add(GKDriversHelper.GK_UID);

            return xDriver;
        }
    }
}