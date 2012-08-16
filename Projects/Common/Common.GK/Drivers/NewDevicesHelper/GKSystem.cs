using System;
using XFiresecAPI;

namespace Common.GK
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