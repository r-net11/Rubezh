using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FiresecAPI.Models;
using Device = FiresecAPI.Models.Device;

namespace ServerFS2
{
	public static class AutoDetectOperationHelper
	{
		public static Device AutoDetectDevice(Device device)
		{
			var rootDevice = (Device)device.Clone();
			var usbDevice = new Device();
			if (rootDevice.Driver.DriverType == DriverType.Computer)
			{
				USBManager.Dispose();
				rootDevice.Children = new List<Device>();

				var usbHidInfos = USBDetectorHelper.FindAllUsbHidInfo();
				foreach (var usbHidInfo in usbHidInfos)
				{
					usbDevice.Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == usbHidInfo.USBDriverType);
					usbDevice.DriverUID = usbDevice.Driver.UID;
					AddChanelToMS(usbDevice);
					rootDevice.Children.Add(usbDevice);
					usbHidInfo.UsbHid.Dispose();
				}
				USBManager.Initialize();
			}
			if ((rootDevice.Driver.DriverType == DriverType.MS_1) || (rootDevice.Driver.DriverType == DriverType.MS_2))
			{
				for (byte sleifNo = 0; sleifNo < rootDevice.Children.Count; sleifNo++)
				{
					rootDevice.Children[sleifNo].Children = new List<Device>();
					for (byte deviceNo = 1; deviceNo < 128; deviceNo++)
					{
						var tempDevice = new Device();
						tempDevice.Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Rubezh_2AM);
						tempDevice.IntAddress = deviceNo;
						tempDevice.Parent = rootDevice.Children[sleifNo];
						var response = USBManager.Send(tempDevice, 0x3C);
						if (response.FunctionCode == 0x7C) // Если по данному адресу найдено устройство, узнаем тип устройства и его версию ПО
						{
							response = USBManager.Send(tempDevice, 0x01, 0x03);
							var driverUid = DriversHelper.GetDriverUidByType(response.Bytes[0]);
							if (driverUid == null)
								continue;
							tempDevice.Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.UID == driverUid);
							rootDevice.Children[sleifNo].Children.Add(tempDevice);
						}
					}
				}
			}
			if ((rootDevice.Driver.DriverType == DriverType.USB_Channel_1) || (rootDevice.Driver.DriverType == DriverType.USB_Channel_2))
			{
				rootDevice.Children = new List<Device>();
				for (byte deviceNo = 1; deviceNo < 128; deviceNo++)
				{
					var tempDevice = new Device();
					tempDevice.Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Rubezh_2AM);
					tempDevice.IntAddress = deviceNo;
					tempDevice.Parent = rootDevice;
					var response = USBManager.Send(tempDevice, 0x3C);
					if (response.FunctionCode == 0x7C) // Если по данному адресу найдено устройство, узнаем тип устройства и его версию ПО
					{
						response = USBManager.Send(tempDevice, 0x01, 0x03);
						var driverUid = DriversHelper.GetDriverUidByType(response.Bytes[0]);
						if (driverUid == null)
							continue;
						tempDevice.Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.UID == driverUid);
						rootDevice.Children.Add(tempDevice);
					}
				}
			}
			return rootDevice;
		}

		static void AddChanelToMS(Device device)
		{
			if ((device.Driver.DriverType != DriverType.MS_1) && (device.Driver.DriverType != DriverType.MS_2))
				return;
			var usbChannel1Device = new Device();
			var usbChannel2Device = new Device();
			// Добавляем 1-й канал
			usbChannel1Device.DriverUID = new Guid("780DE2E6-8EDD-4CFA-8320-E832EB699544");
			usbChannel1Device.Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.UID == usbChannel1Device.DriverUID);
			usbChannel1Device.IntAddress = 1;
			usbChannel1Device.Children = new List<Device>();

			device.Children.Add(usbChannel1Device);
			if (device.Driver.DriverType == DriverType.MS_2)
			{
				// Добавляем 2-й канал
				usbChannel2Device.DriverUID = new Guid("F36B2416-CAF3-4A9D-A7F1-F06EB7AAA76E");
				usbChannel2Device.Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.UID == usbChannel2Device.DriverUID);
				usbChannel2Device.IntAddress = 2;
				usbChannel1Device.Children = new List<Device>();

				device.Children.Add(usbChannel2Device);
			}
		}
	}
}