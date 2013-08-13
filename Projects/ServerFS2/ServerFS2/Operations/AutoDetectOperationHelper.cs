using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FS2Api;
using ServerFS2.Service;
using Device = FiresecAPI.Models.Device;

namespace ServerFS2
{
	public static class AutoDetectOperationHelper
	{
		public static Device AutoDetectDevice(Device device)
		{
			var rootDevice = CopyDevice(device);
			switch (rootDevice.Driver.DriverType)
			{
				case DriverType.Computer:
					{
						USBManager.Dispose();
						var usbHidInfos = USBDetectorHelper.FindAllUsbHidInfo();
						rootDevice.Children = new List<Device>();
						foreach (var usbHidInfo in usbHidInfos)
						{
							var usbDevice = new Device();
							usbDevice.Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == usbHidInfo.USBDriverType);
							usbDevice.DriverUID = usbDevice.Driver.UID;
							AddChanelToMS(usbDevice);
							rootDevice.Children.Add(usbDevice);
							usbDevice.Parent = rootDevice;
							usbHidInfo.UsbHid.Dispose();
						}
					}
					break;
				case DriverType.MS_1:
				case DriverType.MS_2:
					{
						USBManager.ReInitialize(device);
						foreach (var chanel in rootDevice.Children)
						{
							if (!AddDevicesToChanel(chanel))
								return null;
						}
					}
					break;
				case DriverType.USB_Channel_1:
				case DriverType.USB_Channel_2:
					{
						USBManager.ReInitialize(device.Parent);
						rootDevice.Children = new List<Device>();
						if (!AddDevicesToChanel(rootDevice))
							return null;
					}
					break;
			}
			return rootDevice;
		}
		static void AddChanelToMS(Device device)
		{
			if ((device.Driver.DriverType != DriverType.MS_1) && (device.Driver.DriverType != DriverType.MS_2))
				return;
			// Добавляем 1-й канал
			var usbChannel1Device = new Device();
			usbChannel1Device.Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.USB_Channel_1);
			usbChannel1Device.DriverUID = usbChannel1Device.Driver.UID;
			usbChannel1Device.IntAddress = 1;
			usbChannel1Device.Parent = device;
			device.Children.Add(usbChannel1Device);

			if (device.Driver.DriverType == DriverType.MS_2)
			{
				// Добавляем 2-й канал
				var usbChannel2Device = new Device();
				usbChannel2Device.Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.USB_Channel_2);
				usbChannel2Device.DriverUID = usbChannel2Device.Driver.UID;
				usbChannel2Device.IntAddress = 2;
				usbChannel2Device.Parent = device;
				device.Children.Add(usbChannel2Device);
			}
		}
		public static List<byte> GetAddressListFromChanel(Device chanel)
		{
			var response = USBManager.Send(chanel.Parent, "Запрос адресного листа", 0x01, chanel.IntAddress + 2);
			return response.Bytes;
		}
		static Device CopyDevice(Device device)
		{
			var newDevice = CopyOneDevice(device);
			if (device.Parent != null)
			{
				var newParent = CopyOneDevice(device.Parent);
				newDevice.Parent = newParent;
			}

			foreach (var child in device.Children)
			{
				var newChild = CopyOneDevice(child);
				newChild.Parent = newDevice;
				newDevice.Children.Add(newChild);
			}
			return newDevice;
		}
		static Device CopyOneDevice(Device device)
		{
			var newDevice = new Device();
			newDevice.IntAddress = device.IntAddress;
			newDevice.Driver = device.Driver;
			newDevice.DriverUID = device.DriverUID;
			newDevice.UID = device.UID;
			newDevice.Properties = device.Properties;
			return newDevice;
		}
		static bool AddDevicesToChanel(Device chanel)
		{
			//SetAddressListToChanel(chanel);
			chanel.Children = new List<Device>();
			for (byte deviceNo = 1; deviceNo < 128; deviceNo++)
			{
				CallbackManager.AddProgress(new FS2ProgressInfo("Поиск устройств. Канал: " + chanel.IntAddress + ", Адрес: " + deviceNo));
				var tempDevice = new Device();
				tempDevice.Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Rubezh_2AM);
				tempDevice.IntAddress = deviceNo;
				tempDevice.Parent = chanel;
				var response = USBManager.Send(tempDevice, "Пинг", 0x3C);
				if (response.Bytes == null)
					return false;
				if (response.FunctionCode == 0x7C)
				// Если по данному адресу найдено устройство, узнаем тип устройства и его версию ПО
				{
					response = USBManager.Send(tempDevice, "Запрос типа устройства", 0x01, 0x03);
					var driverUid = DriversHelper.GetDriverUidByType(response.Bytes[0]);
					if (driverUid == Guid.Empty)
						continue;
					tempDevice.Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.UID == driverUid);
					tempDevice.DriverUID = driverUid;
					chanel.Children.Add(tempDevice);
				}
			}
			return true;
		}
	}
}