using System;
using System.Linq;
using GroupControllerModule.Converter;
using GroupControllerModule.Models;

namespace GroupControllerModule.ViewModels
{
    public static class AddressControllerHelper
    {
        public static XDriver Create()
        {
            var xDriver = new XDriver()
            {
                DriverType = XDriverType.AddressController,
                UID = DriversHelper.AddressControllerUID,
                OldDriverUID = Guid.Empty,
                CanEditAddress = false,
                ImageSource = "",
                HasImage = false,
                IsChildAddressReservedRange = false,
                Name = "Контроллер адресных устройств",
                ShortName = "КАУ"
            };
            foreach (var driver in XManager.DriversConfiguration.Drivers)
            {
                if (DriversHelper.Drivers.Any(x=>x.ConnectToAddressController))
                {
                    xDriver.Children.Add(driver.UID);
                }
            }
            return xDriver;
        }
    }
}