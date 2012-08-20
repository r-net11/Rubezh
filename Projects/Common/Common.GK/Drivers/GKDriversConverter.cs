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
					//DriverType = driverItem.XDriverType,
					//DriverTypeNo = driverItem.DriverTypeNo,
					//UID = driver.UID,
					//OldDriverUID = driver.UID,
					//Name = driver.Name,
					//ShortName = driver.ShortName,
					//CanEditAddress = driver.CanEditAddress,
					//CanEditAddress = true,
					IsChildAddressReservedRange = driver.IsChildAddressReservedRange,
					IsAutoCreate = driver.IsAutoCreate,
					//AutoChild = driver.AutoChild,
					AutoChildCount = (byte)driver.AutoChildCount,
					//MinAddress = (byte)driver.MinAutoCreateAddress,
					//MaxAddress = (byte)driver.MaxAutoCreateAddress,
					UseParentAddressSystem = driver.UseParentAddressSystem,
					IsRangeEnabled = driver.IsRangeEnabled,
					//MinAddress = (byte)driver.MinAddress,
					//MaxAddress = (byte)driver.MaxAddress,
					//HasAddress = true,
					//HasAddress = driver.HasAddress,
					ChildAddressReserveRangeCount = (byte)driver.ChildAddressReserveRangeCount,
					//IsDeviceOnShleif = driver.IsDeviceOnShleif,
					//IsDeviceOnShleif = true,
					//HasLogic = driver.IsZoneLogicDevice,
					//HasZone = driver.IsZoneDevice,
					IsGroupDevice = driver.IsChildAddressReservedRange
				};

				//xDriver.Children = new List<Guid>();
				//foreach (var childDriver in driver.AvaliableChildren)
				//{
				//    xDriver.Children.Add(childDriver);
				//}

				//xDriver.AutoCreateChildren = new List<Guid>();
				//foreach (var childDriver in driver.AutoCreateChildren)
				//{
				//    xDriver.AutoCreateChildren.Add(childDriver);
				//}

				XManager.DriversConfiguration.Drivers.Add(xDriver);
			}

			XManager.DriversConfiguration.Drivers.Add(GKSystemHelper.Create());
			XManager.DriversConfiguration.Drivers.Add(GKHelper.Create());
			XManager.DriversConfiguration.Drivers.Add(KAUHelper.Create());
			XManager.DriversConfiguration.Drivers.Add(KAUIndicatorHelper.Create());
			XManager.DriversConfiguration.Drivers.Add(GKIndicatorHelper.Create());
			XManager.DriversConfiguration.Drivers.Add(GKLineHelper.Create());
			XManager.DriversConfiguration.Drivers.Add(GKReleHelper.Create());
			XManager.DriversConfiguration.Drivers.Add(GKReleHelper.Create());

			AddDriverToKau(SmokeDetectorHelper.Create());
			AddDriverToKau(CombinedDetectorHelper.Create());
			AddDriverToKau(HeatDetectorHelper.Create());
			AddDriverToKau(HandDetectorHelper.Create());
			AddDriverToKau(RMHelper.Create());
			AddDriverToKau(AM1Helper.Create());
			AddDriverToKau(MROHelper.Create());
			AddDriverToKau(BUNHelper.Create());
			AddDriverToKau(BUZHelper.Create());
			AddDriverToKau(MDUHelper.Create());
			AddDriverToKau(MPTHelper.Create());
			AddDriverToKau(AMP4Helper.Create());
			AddDriverToKau(RM5Helper.Create());
		}

		static void AddDriverToKau(XDriver driver)
		{
			XManager.DriversConfiguration.Drivers.Add(driver);
			var kauDriver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.KAU);
			kauDriver.Children.Add(driver.DriverType);
		}
	}
}