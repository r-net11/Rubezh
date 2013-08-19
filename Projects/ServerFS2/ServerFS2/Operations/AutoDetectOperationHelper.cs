using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FS2Api;
using ServerFS2.Service;
using Device = FiresecAPI.Models.Device;
using ServerFS2.Processor;

namespace ServerFS2
{
	public static class AutoDetectOperationHelper
	{
		public static Device AutoDetectDevice(Device device)
		{
			try
			{
				if (device.Driver.DriverType == DriverType.Computer)
					MainManager.StopMonitoring();

				var rootDevice = CopyDevice(device);
				switch (rootDevice.Driver.DriverType)
				{
					case DriverType.Computer:
						{
							CallbackManager.AddProgress(new FS2ProgressInfo("Поиск USB устройств"));
							var usbHidInfos = USBDetectorHelper.FindAllUsbHidInfo();
							rootDevice.Children = new List<Device>();
							foreach (var usbHidInfo in usbHidInfos)
							{
								var usbDevice = CreateDevice(usbHidInfo.USBDriverType, 0, rootDevice);
								var serialNoProperty = new Property();
								serialNoProperty.Name = "SerialNo";
								serialNoProperty.Value = usbHidInfo.SerialNo;
								usbDevice.Properties.Add(serialNoProperty);
								AddChanelToMS(usbDevice);
								rootDevice.Children.Add(usbDevice);
								usbHidInfo.UsbHid.Dispose();
							}
						}
						break;
					case DriverType.MS_1:
					case DriverType.MS_2:
						{
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
							rootDevice.Children = new List<Device>();
							if (!AddDevicesToChanel(rootDevice))
								return null;
						}
						break;
				}
				return rootDevice;
			}
			finally
			{
				if (device.Driver.DriverType == DriverType.Computer)
					MainManager.StartMonitoring();
				//else
				//    MainManager.ResumeMonitoring(device);
			}
		}

		static void AddChanelToMS(Device device)
		{
			if ((device.Driver.DriverType != DriverType.MS_1) && (device.Driver.DriverType != DriverType.MS_2))
				return;
			device.Children.Add(CreateDevice(DriverType.USB_Channel_1, 1, device));
			if (device.Driver.DriverType == DriverType.MS_2)
				device.Children.Add(CreateDevice(DriverType.USB_Channel_2, 2, device));
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
			chanel.Children = new List<Device>();
			for (byte deviceNo = 1; deviceNo < 255; deviceNo++)
			{
				CallbackManager.AddProgress(new FS2ProgressInfo(chanel.IntAddress + " - Канал. Поиск PNP-устройств Рубеж с адресом " + deviceNo + ".\nВсего адресов: 255", deviceNo * 100 / 255));
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

		static Device CreateDevice(DriverType driverType, int intAddress, Device parent)
		{
			var newDevice = new Device();
			newDevice.Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == driverType);
			newDevice.DriverUID = newDevice.Driver.UID;
			newDevice.IntAddress = intAddress;
			newDevice.Parent = parent;
			return newDevice;
		}
	}
}