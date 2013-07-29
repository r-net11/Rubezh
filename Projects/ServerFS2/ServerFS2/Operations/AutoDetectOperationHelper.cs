using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using ServerFS2.Processor;
using Device = FiresecAPI.Models.Device;

namespace ServerFS2
{
	public static class AutoDetectOperationHelper
	{
		public static Device AutoDetectDevice(Device device)
		{
			var rootDevice = new Device();
			rootDevice.IntAddress = device.IntAddress;
			rootDevice.Driver = device.Driver;
			rootDevice.DriverUID = device.DriverUID;
			rootDevice.UID = device.UID;
			rootDevice.Properties = device.Properties;
			rootDevice.Children = new List<Device>();
			foreach (var child in device.Children)
			{
				var rootChild = new Device();
				rootChild.IntAddress = child.IntAddress;
				rootChild.Driver = child.Driver;
				rootChild.DriverUID = child.DriverUID;
				rootChild.UID = child.UID;
				rootChild.Properties = child.Properties;
				rootChild.Parent = rootDevice;
				rootDevice.Children.Add(rootChild);
			}
			var usbDevice = new Device();
			if (rootDevice.Driver.DriverType == DriverType.Computer)
			{
				MainManager.StopMonitoring();
				var usbHidInfos = USBDetectorHelper.FindAllUsbHidInfo();
				rootDevice.Children = new List<Device>();
				foreach (var usbHidInfo in usbHidInfos)
				{
					usbDevice.Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == usbHidInfo.USBDriverType);
					usbDevice.DriverUID = usbDevice.Driver.UID;
					AddChanelToMS(usbDevice);
					rootDevice.Children.Add(usbDevice);
					usbHidInfo.UsbHid.Dispose();
				}
				MainManager.StartMonitoring();
			}
			if ((rootDevice.Driver.DriverType == DriverType.MS_1) || (rootDevice.Driver.DriverType == DriverType.MS_2))
			{
				USBManager.ReInitialize(device);
				foreach (var chanel in rootDevice.Children)
				{
					SetAddressListToChanel(chanel);
					chanel.Children = new List<Device>();
					for (byte deviceNo = 1; deviceNo < 128; deviceNo++)
					{
						var tempDevice = new Device();
						tempDevice.Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Rubezh_2AM);
						tempDevice.IntAddress = deviceNo;
						tempDevice.Parent = chanel;
						var response = USBManager.Send(tempDevice, 0x3C);
						if (response.Bytes == null)
							return null;
						if (response.FunctionCode == 0x7C) // Если по данному адресу найдено устройство, узнаем тип устройства и его версию ПО
						{
							response = USBManager.Send(tempDevice, 0x01, 0x03);
							var driverUid = DriversHelper.GetDriverUidByType(response.Bytes[0]);
							if (driverUid == Guid.Empty)
								continue;
							tempDevice.Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.UID == driverUid);
							tempDevice.DriverUID = driverUid;  
							chanel.Children.Add(tempDevice);
						}
					}
				}
			}
			if ((rootDevice.Driver.DriverType == DriverType.USB_Channel_1) || (rootDevice.Driver.DriverType == DriverType.USB_Channel_2))
			{
				USBManager.ReInitialize(device.Parent);
				rootDevice.Parent = new Device();
				rootDevice.Parent.IntAddress = device.Parent.IntAddress;
				rootDevice.Parent.Driver = device.Parent.Driver;
				rootDevice.Parent.DriverUID = device.Parent.DriverUID;
				rootDevice.Parent.UID = device.Parent.UID;
				rootDevice.Parent.Properties = device.Parent.Properties;

				SetAddressListToChanel(rootDevice);
				rootDevice.Children = new List<Device>();
				for (byte deviceNo = 1; deviceNo < 128; deviceNo++)
				{
					var tempDevice = new Device();
					tempDevice.Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Rubezh_2AM);
					tempDevice.IntAddress = deviceNo;
					tempDevice.Parent = rootDevice;
					var response = USBManager.Send(tempDevice, 0x3C);
					if (response.Bytes == null)
						return null;
					if (response.FunctionCode == 0x7C) // Если по данному адресу найдено устройство, узнаем тип устройства и его версию ПО
					{
						response = USBManager.Send(tempDevice, 0x01, 0x03);
						var driverUid = DriversHelper.GetDriverUidByType(response.Bytes[0]);
						if (driverUid == Guid.Empty)
							continue;
						tempDevice.Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.UID == driverUid);
						tempDevice.DriverUID = driverUid;  
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

		public static List<byte> GetAddressListFromChanel(Device chanel)
		{
			var response = USBManager.Send(chanel.Parent, 0x01, chanel.IntAddress + 2);
			return response.Bytes;
		}

		public static void SetAddressListToChanel(Device chanel)
		{
			var bytes = new List<byte>();
			var chanelAddress = Convert.ToByte(chanel.Properties.FirstOrDefault(x => x.Name == "Address").Value);
			var baudRate = Convert.ToByte(chanel.Parent.Properties.FirstOrDefault(x => x.Name == "BaudRate").Value);
			bytes.Add(chanelAddress);
			foreach (var child in chanel.Children)
			{
				bytes.Add((byte)child.AddressOnShleif);
			}
			bytes.Sort();
			int nullCount = 32 - bytes.Count;
			for (int i = 0; i < nullCount; i++)
				bytes.Add(0x00);
			USBManager.Send(chanel.Parent, 0x02, chanel.IntAddress + 2, chanelAddress, baudRate, bytes);
		}
	}
}