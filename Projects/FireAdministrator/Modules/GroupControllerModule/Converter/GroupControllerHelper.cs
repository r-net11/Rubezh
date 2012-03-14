using System;
using GroupControllerModule.Converter;
using GroupControllerModule.Models;

namespace GroupControllerModule.ViewModels
{
    public static class GroupControllerHelper
    {
        public static XDriver Create()
        {
            var xDriver = new XDriver()
            {
                DriverType = XDriverType.GroupController,
                UID = DriversHelper.GroupControllerUID,
                OldDriverUID = Guid.Empty,
                CanEditAddress = false,
                ImageSource = "",
                HasImage = false,
                IsChildAddressReservedRange = false,
                Name = "Групповой контроллер",
                ShortName = "ГК"
            };
            xDriver.Children.Add(DriversHelper.AddressControllerUID);
            return xDriver;
        }
    }
}