using System;
using RubezhAPI.GK;

namespace GKProcessor
{
    public static class MultiGK_Helper
    {
        public static GKDriver Create()
        {
            var driver = new GKDriver()
            {
                DriverType = GKDriverType.MultiGK,
                UID = new Guid("5C7DCFD6-9728-43B1-A94B-573AB4B5F336"),
                Name = "Групповой контроллер составной",
                ShortName = "Составной ГК",
                HasAddress = false,
                IsDeviceOnShleif = false,
                IsPlaceable = false,
                IsReal = false,
                GroupDeviceChildrenCount = 2
            };
            driver.Children.Add(GKDriverType.RSR2_KAU);
            driver.AvailableStateClasses.Add(XStateClass.Norm);
            driver.AvailableStateClasses.Add(XStateClass.Unknown);
            driver.AvailableStateClasses.Add(XStateClass.Failure);
            return driver;
        }
    }
}
