using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GroupControllerModule.Models;
using GroupControllerModule.Converter;

namespace GroupControllerModule.ViewModels
{
    public static class AddressControllerHelper
    {
        public static GCDriver Create()
        {
            var gCDriver = new GCDriver()
            {
                DriverType = GCDriverType.AddressController,
                UID = DriversHelper.AddressControllerUID,
                OldDriverUID = Guid.Empty,
                CanEditAddress = false,
                ImageSource = "",
                HasImage = false,
                IsChildAddressReservedRange = false,
                Name = "Контроллер адресных устройств",
                ShortName = "КАУ"
            };
            foreach (var driver in ConfigurationConverter.GCDriversConfiguration.Drivers)
            {
                if (DriversHelper.Drivers.Any(x=>x.ConnectToAddressController))
                {
                    gCDriver.Children.Add(driver.UID);
                }
            }
            return gCDriver;
        }
    }
}