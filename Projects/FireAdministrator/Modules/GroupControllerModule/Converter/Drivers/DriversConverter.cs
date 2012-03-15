using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using GroupControllerModule.Models;

namespace GroupControllerModule.Converter
{
    public static class DriversConverter
    {
        public static void Convert()
        {
            XManager.DriversConfiguration = new XDriversConfiguration();

            foreach (var driver in FiresecManager.Drivers)
            {
                var driverItem = DriversHelper.Drivers.FirstOrDefault(x => x.OldDriverType == driver.DriverType);
                if (driverItem == null)
                    continue;

                var xDriver = new XDriver()
                {
                    DriverType = driverItem.XDriverType,
                    UID = driver.UID,
                    OldDriverUID = driver.UID,
                    Name = driver.Name,
                    ShortName = driver.ShortName,
                    ImageSource = driver.ImageSource,
                    HasImage = driver.HasImage,
                    CanEditAddress = driver.CanEditAddress,
                    IsChildAddressReservedRange = driver.IsChildAddressReservedRange,
                    IsAutoCreate = driver.IsAutoCreate,
                    AutoChild = driver.AutoChild,
                    AutoChildCount = driver.AutoChildCount,
                    MinAutoCreateAddress = driver.MinAutoCreateAddress,
                    MaxAutoCreateAddress = driver.MaxAutoCreateAddress,
                    UseParentAddressSystem = driver.UseParentAddressSystem,
                    IsRangeEnabled = driver.IsRangeEnabled,
                    MinAddress = driver.MinAddress,
                    MaxAddress = driver.MaxAddress,
                    HasAddress = driver.HasAddress,
                    ChildAddressReserveRangeCount = driver.ChildAddressReserveRangeCount
                };

                xDriver.Children = new List<Guid>();
                foreach (var childDriver in driver.AvaliableChildren)
                {
                    xDriver.Children.Add(childDriver);
                }

                xDriver.AutoCreateChildren = new List<Guid>();
                foreach (var childDriver in driver.AutoCreateChildren)
                {
                    xDriver.AutoCreateChildren.Add(childDriver);
                }

                XManager.DriversConfiguration.Drivers.Add(xDriver);
            }

            XManager.DriversConfiguration.Drivers.Add(GCHelper.Create());
            XManager.DriversConfiguration.Drivers.Add(KAUHelper.Create());
            XManager.DriversConfiguration.Drivers.Add(KAUIndicatorHelper.Create());
            XManager.DriversConfiguration.Drivers.Add(KAUExitHelper.Create());
        }
    }
}