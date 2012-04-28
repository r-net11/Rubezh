using System;
using XFiresecAPI;

namespace GKModule.Converter
{
    public static class GKSystemHelper
    {
        public static XDriver Create()
        {
            var xDriver = new XDriver()
            {
                DriverType = XDriverType.System,
                UID = DriversHelper.System_UID,
                OldDriverUID = Guid.Empty,
                CanEditAddress = false,
                HasAddress = false,
                ImageSource = FiresecClient.FileHelper.GetIconFilePath("Device_Device.ico"),
                HasImage = true,
                IsChildAddressReservedRange = false,
                Name = "Система",
                ShortName = "Система"
            };
            xDriver.Children.Add(DriversHelper.GK_UID);

            return xDriver;
        }
    }
}