using System;
using System.Collections.Generic;
using System.Linq;
using Common.GK;
using FiresecAPI.Models;
using FiresecClient;
using XFiresecAPI;
using System.Diagnostics;

namespace GKModule.Converter
{
    public class ConfigurationConverter2
    {
        XDevice gkDevice;

        public void Convert()
        {
			ConvertDdevices();
			ConvertZones();
		}

		public void ConvertDdevices()
		{
            AddRootDevices();

            int shleifPairNo = 0;
            byte kauAddress = 1;
            XDevice curentKauDevice = null;

            foreach (var panelDevice in GetPanels())
            {
				shleifPairNo++;
				if (shleifPairNo == 5)
					shleifPairNo = 1;

                if (shleifPairNo == 1)
                {
                    curentKauDevice = XManager.AddChild(gkDevice, XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.KAU), 0, kauAddress);
                    curentKauDevice.UID = panelDevice.UID;
                    XManager.DeviceConfiguration.Devices.Add(curentKauDevice);
                }

                foreach (var childDevice in panelDevice.Children)
                {
                    var driver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.UID == childDevice.DriverUID);
                    if (driver == null)
                    {
                        continue;
                    }
					var shleifNo = ((shleifPairNo - 1) * 2) + (childDevice.IntAddress >> 8);

					switch(driver.DriverType)
					{
						case XDriverType.RM_2:
						case XDriverType.RM_3:
						case XDriverType.RM_4:
						case XDriverType.RM_5:
						case XDriverType.AM4:
							continue;

						default:
							break;
					}

                    var xDevice = new XDevice()
                    {
                        UID = childDevice.UID,
                        DriverUID = driver.UID,
                        Driver = driver,
						ShleifNo = (byte)shleifNo,
                        IntAddress = (byte)(childDevice.IntAddress & 0xff),
                        Description = childDevice.Description
                    };
                    XManager.DeviceConfiguration.Devices.Add(xDevice);
                    curentKauDevice.Children.Add(xDevice);
                    xDevice.Parent = curentKauDevice;
                }
            }
        }

        public void AddRootDevices()
        {
            XManager.DeviceConfiguration = new XDeviceConfiguration();

            var systemDevice = new XDevice()
            {
                UID = Guid.NewGuid(),
                DriverUID = GKDriversHelper.System_UID,
                Driver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.System)
            };
            XManager.DeviceConfiguration.Devices.Add(systemDevice);
            XManager.DeviceConfiguration.RootDevice = systemDevice;

            var gkDriver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.GK);
            gkDevice = XManager.AddChild(systemDevice, gkDriver, 0, 1);
            gkDevice.UID = Guid.NewGuid();
            XManager.DeviceConfiguration.Devices.Add(gkDevice);
        }

        IEnumerable<Device> GetPanels()
        {
            foreach (var device in FiresecManager.Devices)
            {
                if (device.Driver.DeviceClassName == "ППКП")
                    yield return device;
            }
        }

		void ConvertZones()
		{
			foreach (var zone in FiresecManager.Zones)
			{
				var xZone = new XZone()
				{
					No = (ushort)zone.No,
					Name = zone.Name,
					Description = zone.Description,
					Fire1Count = (ushort)zone.DetectorCount,
				};
				XManager.DeviceConfiguration.Zones.Add(xZone);
			}

			foreach (var device in FiresecManager.Devices)
			{
				var xDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == device.UID);
				if (xDevice != null)
				{
					if ((device.Driver.IsZoneDevice) && (device.ZoneNo.HasValue))
					{
						var zone = FiresecManager.Zones.FirstOrDefault(x => x.No == device.ZoneNo.Value);
						var xZone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == (ushort)zone.No);
						if (zone != null)
						{
							xDevice.Zones.Add(xZone.No);
							xZone.DeviceUIDs.Add(device.UID);
						}
					}
				}
			}
		}
    }
}