using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using UsbLibrary;
using System.Threading;

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

		public static Response SendWithoutException(Device device, params object[] value)
		{
			return Send(device, true, value);
		}

		public static Response Send(Device device, params object[] value)
		{
			return Send(device, false, value);
		}

		static Response Send(Device device, bool throwException, params object[] value)
		{
			var usbProcessor = GetUsbProcessor(device);
			if (usbProcessor != null)
			{
				var bytes = CreateBytesArray(value);
				var outputFunctionCode = Convert.ToByte(bytes[0]);
				bytes = CreateOutputBytes(device, usbProcessor.WithoutId, bytes);
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
						if (throwException)
							throw new FS2USBException("Недостаточное количество байт в ответе");
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
						if (throwException)
							throw new FS2USBException(errorName);
						return response.SetError(errorName);
					}
					if ((functionCode & 64) != 64)
					{
						if (throwException)
							throw new FS2USBException("В пришедшем ответе не содержится маркер ответа");
						return response.SetError("В пришедшем ответе не содержится маркер ответа");
					}

					if ((functionCode & 63) != outputFunctionCode)
					{
						if (throwException)
							throw new FS2USBException("В пришедшем ответе не совпадает код функции");
						return response.SetError("В пришедшем ответе не совпадает код функции");
					}

					response.Bytes.RemoveRange(0, 1);
					return response;
				}
				if (throwException)
					throw new FS2USBException("Не получен ответ в заданное время");
				return new Response("Не получен ответ в заданное время");
			}
			else
			{
				if (throwException)
					throw new FS2USBException("USB устройство отсутствует");
				return new Response("USB устройство отсутствует");
			}
		}

		public static int SendAsync(Device device, params object[] value)
		{
			var usbProcessor = GetUsbProcessor(device);
			if (usbProcessor != null)
			{
				var bytes = CreateBytesArray(value);
				bytes = CreateOutputBytes(device, usbProcessor.WithoutId, bytes);
				var requestNo = NextRequestNo;
				usbProcessor.AddRequest(requestNo, new List<List<byte>> { bytes }, 1000, 1000, false);
				return requestNo;
			}
			else
			{
				return -1;
			}
		}

		static List<byte> CreateOutputBytes(Device device, bool withoutId, List<byte> bytes)
		{
			var parentPanel = device.ParentPanel;
			var parentUSB = device.ParentUSB;

			var addressBytes = new List<byte>();
			if (!withoutId)
			{
				if (device.Driver.DriverType == DriverType.MS_1 || device.Driver.DriverType == DriverType.MS_2)
				{
					addressBytes.Add(0x01);
				}
				else
				{
					addressBytes.Add((byte)(parentPanel.Parent.IntAddress + 2));
					addressBytes.Add((byte)parentPanel.IntAddress);
				}
			}
			else
			{
				addressBytes.Add(0x02);
			}
			bytes.InsertRange(0, addressBytes);

			return bytes;
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
			Dispose();
			UsbProcessorInfos = USBDetectorHelper.Detect();
			foreach (var usbProcessorInfo in UsbProcessorInfos)
			{
				usbProcessorInfo.UsbProcessor.DeviceRemoved += new Action<UsbProcessor>(UsbProcessor_DeviceRemoved);
			}
		}

		public static List<string> GetAllSerialNos()
		{
			Dispose();
			var result = new List<string>();
			var usbProcessorInfos = USBDetectorHelper.FindAllUsbProcessorInfo();
			foreach (var usbProcessorInfo in usbProcessorInfos)
			{
				result.Add(usbProcessorInfo.SerialNo);
				usbProcessorInfo.UsbProcessor.Dispose();
			}
			Thread.Sleep(TimeSpan.FromSeconds(5));
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
			if (UsbProcessorInfos != null)
			{
				UsbProcessorInfos.ForEach(x => x.UsbProcessor.Dispose());
				UsbProcessorInfos.Clear();
			}
		}
	}
}