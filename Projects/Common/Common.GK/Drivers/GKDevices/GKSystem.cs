using System;
using XFiresecAPI;

namespace Common.GK
{
    public static class GKSystemHelper
    {
        public static XDriver Create()
        {
            var driver = new XDriver()
            {
                DriverType = XDriverType.System,
                UID = GKDriversHelper.System_UID,
                Name = "Система",
                ShortName = "Система",
                CanEditAddress = false,
                HasAddress = false,
				IsDeviceOnShleif = false,
                IsChildAddressReservedRange = false,
            };
			driver.Children.Add(XDriverType.GK);

            return driver;
        }
    }
}