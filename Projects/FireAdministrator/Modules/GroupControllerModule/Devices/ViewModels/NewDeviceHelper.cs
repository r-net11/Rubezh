using System;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;

namespace GKModule.ViewModels
{
	public static class NewDeviceHelper
	{
		public static byte GetMinAddress(GKDriver driver, GKDevice parentDevice)
		{
			byte maxAddress = 0;

			if (driver.IsRangeEnabled)
			{
				maxAddress = driver.MinAddress;
			}
			else
			{
				if (parentDevice.Driver.IsGroupDevice)
				{
					maxAddress = (byte)parentDevice.IntAddress;
				}
			}

			foreach (var child in parentDevice.Children)
			{
				if (child.Driver.IsAutoCreate)
					continue;

				if (driver.IsRangeEnabled)
				{
					if ((child.IntAddress < driver.MinAddress) && (child.IntAddress > driver.MaxAddress))
						continue;
				}

				if (child.Driver.IsGroupDevice)
				{
					if (child.IntAddress + child.Driver.GroupDeviceChildrenCount - 1 > maxAddress)
						maxAddress = (byte)Math.Min(255, child.IntAddress + child.Driver.GroupDeviceChildrenCount - 1);
				}

				if (child.IntAddress > maxAddress)
					maxAddress = (byte)child.IntAddress;
			}

			if (driver.IsRangeEnabled)
			{
				if (parentDevice.Children.Where(x => x.Driver.IsAutoCreate == false).Count() > 0)
					if (maxAddress + 1 <= driver.MaxAddress)
						maxAddress = (byte)(maxAddress + 1);
			}
			else
			{
				if (parentDevice.Driver.IsGroupDevice)
				{
					if (parentDevice.Children.Count > 0)
						if (maxAddress + 1 <= parentDevice.IntAddress + driver.GroupDeviceChildrenCount - 1)
							maxAddress = (byte)(maxAddress + 1);
				}
				else
				{
					if (parentDevice.Children.Where(x => x.Driver.IsAutoCreate == false).ToList().Count > 0)
						if (((maxAddress + 1) % 256) != 0)
							maxAddress = (byte)(maxAddress + 1);
				}
			}

			return Math.Max((byte)1, maxAddress);
		}
	}
}