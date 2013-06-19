using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using UsbLibrary;

namespace ServerFS2
{
	public static class USBManager
	{
		static int RequestNo = 0;
		public static int NextRequestNo
		{
			get { return RequestNo++; }
		}

		public static List<UsbProcessorInfo> UsbProcessorInfos { get; set; }

		public static Response SendCodeToPanel(Device device, params object[] value)
		{
			var usbProcessor = GetUsbProcessor(device);
			if (usbProcessor != null)
			{
				var bytes = CreateBytesArray(value);
				var outputFunctionCode = Convert.ToByte(bytes[0]);
				bytes.InsertRange(0, usbProcessor.WithoutId ? new List<byte> { (byte)(0x02) } : ((device.Driver.DriverType == DriverType.MS_1) || (device.Driver.DriverType == DriverType.MS_2)) ? new List<byte> { (byte)(0x01) } : new List<byte> { (byte)(device.Parent.IntAddress + 2), (byte)device.IntAddress });
				var response = usbProcessor.AddRequest(NextRequestNo, new List<List<byte>> { bytes }, 1000, 1000, true);
				if (response != null)
				{
					var inputBytes = response.Bytes.ToList();
					if (!usbProcessor.WithoutId)
					{
						response.Bytes.RemoveRange(0, 4);
					}
					if (usbProcessor.WithoutId)
					{
						var usbRoot = response.Bytes[0];
						response.Bytes.RemoveRange(0, 1);
					}
					else
					{
						var usbRoot = response.Bytes[0];
						var panelRoot = response.Bytes[1];
						response.Bytes.RemoveRange(0, 2);
					}

					if (response.Bytes.Count < 1)
					{
						return response.SetError("Недостаточное количество байт в ответе");
					}
					var functionCode = response.Bytes[0];
					if ((functionCode & 128) == 128)
					{
						var errorName = "В ответе содержится код ошибки";
						if (response.Bytes.Count >= 2)
						{
							errorName = USBExceptionHelper.GetError(response.Bytes[1]);
						}
						return response.SetError(errorName);
					}
					if ((functionCode & 64) != 64)
					{
						return response.SetError("В пришедшем ответе не содержится маркер ответа");
					}
					if ((functionCode & 63) != outputFunctionCode)
					{
						return response.SetError("В пришедшем ответе не совпадает код функции");
					}

					response.Bytes.RemoveRange(0, 1);
					return response;
				}
				return new Response("Не получен ответ в заданное время");
			}
			else
			{
				throw new FS2USBException("USB устройство отсутствует");
				return new Response("USB устройство отсутствует");
			}
		}

		public static int SendCodeToPanelAsync(Device device, params object[] value)
		{
			var usbProcessor = GetUsbProcessor(device);
			if (usbProcessor != null)
			{
				var bytes = CreateBytesArray(value);
				bytes.InsertRange(0, usbProcessor.WithoutId ? new List<byte> { (byte)(0x02) } : new List<byte> { (byte)(device.Parent.IntAddress + 2), (byte)device.IntAddress });
				var requestNo = NextRequestNo;
				usbProcessor.AddRequest(requestNo, new List<List<byte>> { bytes }, 1000, 1000, false);
				return requestNo;
			}
			else
			{
				return -1;
			}
		}

		public static bool IsUsbDevice(Device device)
		{
			var usbProcessor = GetUsbProcessor(device);
			if (usbProcessor != null)
			{
				return usbProcessor.WithoutId;
			}
			return false;
		}

		public static UsbProcessor GetUsbProcessor(Device panelDevice)
		{
			var parentUSB = panelDevice.ParentUSB;
			if (parentUSB != null)
			{
				var usbProcessorInfo = UsbProcessorInfos.FirstOrDefault(x => x.Device.UID == parentUSB.UID);
				if (usbProcessorInfo != null)
				{
					return usbProcessorInfo.UsbProcessor;
				}
			}
			return null;
		}

		public static List<byte> CreateBytesArray(params object[] values)
		{
			var bytes = new List<byte>();
			foreach (var value in values)
			{
				if (value as IEnumerable<Byte> != null)
					bytes.AddRange((IEnumerable<Byte>)value);
				else
					bytes.Add(Convert.ToByte(value));
			}
			return bytes;
		}

		public static void Initialize()
		{
			var usbDevices = new List<Device>();
			foreach (var device in ConfigurationManager.DeviceConfiguration.RootDevice.Children)
			{
				switch (device.Driver.DriverType)
				{
					case DriverType.MS_1:
					case DriverType.MS_2:
					case DriverType.USB_Rubezh_2AM:
					case DriverType.USB_Rubezh_2OP:
					case DriverType.USB_Rubezh_4A:
					case DriverType.USB_Rubezh_P:
					case DriverType.USB_BUNS:
					case DriverType.USB_BUNS_2:
						usbDevices.Add(device);
						break;
				}
			}

			UsbProcessorInfos = FindAllUsbProcessorInfo();

			foreach (var device in usbDevices)
			{
				UsbProcessorInfo usbProcessorInfo = null;
				var serialNoProperty = device.Properties.FirstOrDefault(x => x.Name == "SerialNo");
				//var serialNoProperty = device.Properties.FirstOrDefault();
				if (serialNoProperty != null && !string.IsNullOrEmpty(serialNoProperty.Value))
				{
					usbProcessorInfo = UsbProcessorInfos.FirstOrDefault(x => x.SerialNo == serialNoProperty.Value);
					if (usbProcessorInfo != null)
					{
						usbProcessorInfo.Device = device;
						continue;
					}
				}

				var driverTypeNo = DriversHelper.GetTypeNoByDriverType(device.Driver.DriverType);
				usbProcessorInfo = UsbProcessorInfos.FirstOrDefault(x => x.TypeNo == driverTypeNo);
				if (usbProcessorInfo != null)
				{
					usbProcessorInfo.Device = device;
					continue;
				}
			}

			foreach (var device in usbDevices)
			{
				if (device.Driver.DriverType == DriverType.MS_1 || device.Driver.DriverType == DriverType.MS_2)
				{
					var serialNoProperty = device.Properties.FirstOrDefault(x => x.Name == "SerialNo");
					if (serialNoProperty == null || string.IsNullOrEmpty(serialNoProperty.Value))
					{
						var usbProcessorInfo = UsbProcessorInfos.FirstOrDefault(x => x.TypeNo == -1 && x.Device == null);
						if (usbProcessorInfo != null)
						{
							usbProcessorInfo.Device = device;
						}
					}
				}
			}

			foreach (var usbProcessorInfo in UsbProcessorInfos)
			{
				if (usbProcessorInfo.Device == null)
					usbProcessorInfo.UsbProcessor.Dispose();
			}
			UsbProcessorInfos.RemoveAll(x => x.Device == null);

			foreach (var usbProcessorInfo in UsbProcessorInfos)
			{
				switch (usbProcessorInfo.Device.Driver.DriverType)
				{
					case DriverType.MS_1:
					case DriverType.MS_2:
						usbProcessorInfo.WriteConfigToMS();
						break;
				}
			}

			foreach (var usbProcessorInfo in UsbProcessorInfos)
			{
				usbProcessorInfo.UsbProcessor.DeviceRemoved += new Action<UsbProcessor>(UsbProcessor_DeviceRemoved);
			}
		}

		static List<UsbProcessorInfo> FindAllUsbProcessorInfo()
		{
			var usbProcessorInfos = new List<UsbProcessorInfo>();

			HIDDevice.AddedDevices = new List<string>();
			while (true)
			{
				try
				{
					var usbProcessor = new UsbProcessor();
					var result = usbProcessor.Open();
					if (!result)
						break;
					var usbProcessorInfo = new UsbProcessorInfo()
					{
						UsbProcessor = usbProcessor
					};
					usbProcessorInfos.Add(usbProcessorInfo);
				}
				catch (Exception)
				{
					break;
				}
			}

			foreach (var usbProcessorInfo in usbProcessorInfos)
			{
				usbProcessorInfo.Initialize();
				Trace.WriteLine(usbProcessorInfo.IsUSBMS + " " + usbProcessorInfo.IsUSBPanel + " " +
					usbProcessorInfo.USBDriverType + " " + usbProcessorInfo.SerialNo);
			}
			return usbProcessorInfos;
		}

		public static List<string> GetAllSerialNos()
		{
			var result = new List<string>();
			var usbProcessorInfos = FindAllUsbProcessorInfo();
			foreach (var usbProcessorInfo in usbProcessorInfos)
			{
				result.Add(usbProcessorInfo.SerialNo);
			}
			return result;
		}

		static void UsbProcessor_DeviceRemoved(UsbProcessor usbProcessor)
		{
			var usbProcessorInfo = UsbProcessorInfos.FirstOrDefault(x => x.UsbProcessor == usbProcessor);
			if (usbProcessorInfo != null)
			{
				UsbProcessorInfos.Remove(usbProcessorInfo);
			}
		}

		public static void Dispose()
		{
			UsbProcessorInfos.ForEach(x=>x.UsbProcessor.Dispose());
			UsbProcessorInfos.Clear();
		}
	}
}