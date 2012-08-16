using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using XFiresecAPI;

namespace Common.GK
{
	public static class GKDriversConverter
	{
		public static void Convert()
		{
			XManager.DriversConfiguration = new XDriversConfiguration();

			foreach (var driver in FiresecManager.Drivers)
			{
				var driverItem = GKDriversHelper.Drivers.FirstOrDefault(x => x.OldDriverType == driver.DriverType);
				if (driverItem == null)
					continue;

				var xDriver = new XDriver()
				{
					DriverType = driverItem.XDriverType,
					DriverTypeNo = driverItem.DriverTypeNo,
					UID = driver.UID,
					OldDriverUID = driver.UID,
					Name = driver.Name,
					ShortName = driver.ShortName,
					HasImage = driver.HasImage,
					//CanEditAddress = driver.CanEditAddress,
					CanEditAddress = true,
					IsChildAddressReservedRange = driver.IsChildAddressReservedRange,
					IsAutoCreate = driver.IsAutoCreate,
					AutoChild = driver.AutoChild,
					AutoChildCount = (byte)driver.AutoChildCount,
					MinAutoCreateAddress = (byte)driver.MinAutoCreateAddress,
					MaxAutoCreateAddress = (byte)driver.MaxAutoCreateAddress,
					UseParentAddressSystem = driver.UseParentAddressSystem,
					IsRangeEnabled = driver.IsRangeEnabled,
					MinAddress = (byte)driver.MinAddress,
					MaxAddress = (byte)driver.MaxAddress,
					HasAddress = true,
					//HasAddress = driver.HasAddress,
					ChildAddressReserveRangeCount = (byte)driver.ChildAddressReserveRangeCount,
					//IsDeviceOnShleif = driver.IsDeviceOnShleif,
					IsDeviceOnShleif = true,
					HasLogic = driver.IsZoneLogicDevice,
					HasZone = driver.IsZoneDevice,
					IsGroupDevice = driver.IsChildAddressReservedRange
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

			XManager.DriversConfiguration.Drivers.Add(GKSystemHelper.Create());
			XManager.DriversConfiguration.Drivers.Add(GKHelper.Create());
			XManager.DriversConfiguration.Drivers.Add(KAUHelper.Create());
			XManager.DriversConfiguration.Drivers.Add(KAUIndicatorHelper.Create());
			XManager.DriversConfiguration.Drivers.Add(GKIndicatorHelper.Create());
			XManager.DriversConfiguration.Drivers.Add(GKLineHelper.Create());
			XManager.DriversConfiguration.Drivers.Add(GKReleHelper.Create());
			CreateKnownProperties();

            foreach (var driver in XManager.DriversConfiguration.Drivers)
            {
                driver.ImageSource = IconHelper.GetGKIcon(driver);
            }

            foreach (var driver in XManager.DriversConfiguration.Drivers)
            {
                switch (driver.DriverType)
                {
                    case XDriverType.RM_1:
                    case XDriverType.MPT:
                    case XDriverType.Pump:
                    case XDriverType.MRO:
                    case XDriverType.Valve:
                    case XDriverType.MDU:
                        driver.IsControlDevice = true;
                        break;
                }
            }
		}

		static void CreateKnownProperties()
		{
			foreach (var driverType in new List<XDriverType>() { XDriverType.RM_1, XDriverType.RM_2, XDriverType.RM_3, XDriverType.RM_4, XDriverType.RM_5 })
			{
				var xDriver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == driverType);
				RMHelper.Create(xDriver);
			}
			MROHelper.Create();
			AMP4Helper.Create();
			MDUHelper.Create();
			BUZHelper.Create();
			BUNHelper.Create(XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.Pump));
			//foreach (var driverType in new List<XDriverType>() { XDriverType.Pump, XDriverType.JokeyPump, XDriverType.Compressor, XDriverType.DrenazhPump, XDriverType.CompensationPump })
			//{
			//    var xDriver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == driverType);
			//    BUNHelper.Create(xDriver);
			//}
			MPTHelper.Create();
			SmokeDetectorHelper.Create();
			HeatDetectorHelper.Create();
			CombinedDetectorHelper.Create();
			AM1Helper.Create();
			//AMsHelper.Create();
			PumpsHelper.Create();
		}
	}
}