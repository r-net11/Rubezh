using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GroupControllerModule.Models;
using GroupControllerModule.Converter;

namespace GroupControllerModule.ViewModels
{
    public static class GroupControllerHelper
    {
        public static GCDriver Create()
        {
            var gCDriver = new GCDriver()
            {
                DriverType = GCDriverType.GroupController,
                UID = DriversHelper.GroupControllerUID,
                OldDriverUID = Guid.Empty,
                CanEditAddress = false,
                ImageSource = "",
                HasImage = false,
                IsChildAddressReservedRange = false,
                Name = "Групповой контроллер",
                ShortName = "ГК"
            };
            gCDriver.Children.Add(DriversHelper.AddressControllerUID);
            return gCDriver;
        }
    }
}