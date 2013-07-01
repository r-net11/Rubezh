using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using UsbLibrary;
using System.Threading;
using System.Windows.Forms;

namespace ServerFS2
{
	public static class USBManager
	{
		static int RequestNo = 1;
		public static int NextRequestNo
		{
			get { return RequestNo++; }
		}

		public static List<UsbHidInfo> UsbHidInfos { get; set; }

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
			var usbHid = GetUsbUsbHid(device);
			if (usbHid != null)
			{
				var bytes = CreateBytesArray(value);
				var outputFunctionCode = Convert.ToByte(bytes[0]);
				var rootBytes = CreateRootBytes(device, usbHid.UseId);
				bytes.InsertRange(0, rootBytes);

				var response = usbHid.AddRequest(NextRequestNo, new List<List<byte>> { bytes }, 1000, 1000, true);
				if (response != null)
				{
					var inputBytes = response.Bytes.ToList();
					if (usbHid.UseId)
					{
						response.Bytes.RemoveRange(0, 4);
						if (device.Driver.DriverType == DriverType.MS_1 || device.Driver.DriverType == DriverType.MS_2)
						{
							var usbRoot = response.Bytes[0];
							response.Bytes.RemoveRange(0, 1);
							if (usbRoot != rootBytes[0])
							{
								if (throwException)
									throw new FS2USBException("В ответе не совпадает первый байт маршрута");
								return response.SetError("В ответе не совпадает первый байт маршрута");
							}
						}
						else
						{
							var usbRoot = response.Bytes[0];
							var panelRoot = response.Bytes[1];
						response.MsFlag = panelRoot;
							response.Bytes.RemoveRange(0, 2);

							if (usbRoot != rootBytes[0])
							{
								if (throwException)
									throw new FS2USBException("В ответе не совпадает первый байт маршрута");
								return response.SetError("В ответе не совпадает первый байт маршрута");
							}

							if (panelRoot != rootBytes[1])
							{
								if (throwException)
									throw new FS2USBException("В ответе не совпадает второй байт маршрута");
								return response.SetError("В ответе не совпадает второй байт маршрута");
							}
						}
					}
					else
					{
						var usbRoot = response.Bytes[0];
						response.Bytes.RemoveRange(0, 1);
					}

					if (response.Bytes.Count < 1)
					{
						if (throwException)
							throw new FS2USBException("Недостаточное количество байт в ответе");
						return response.SetError("Недостаточное количество байт в ответе");
					}
					var functionCode = response.Bytes[0];
					response.FunctionCode = functionCode;
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
				Initialize();
				if (throwException)
					throw new FS2USBException("USB устройство отсутствует");
				return new Response("USB устройство отсутствует");
			}
		}

		public static void SendAsync(Device device, Request request)
		{
			var usbHid = GetUsbUsbHid(device);
			if (usbHid != null)
			{
				var bytes = request.Bytes.ToList();
				var rootBytes = CreateRootBytes(device, usbHid.UseId);
				bytes.InsertRange(0, rootBytes);
				var requestNo = NextRequestNo;
				if (usbHid.UseId)
				{
					request.Id = requestNo;
				}
				else
				{
					request.Id = 0;
				}
				request.RootBytes = rootBytes;
				usbHid.AddRequest(requestNo, new List<List<byte>> { bytes }, 1000, 1000, false);
			}
			else
			{
				request.Id = -1;
			}
		}

		static List<byte> CreateRootBytes(Device device, bool useId)
		{
			var parentPanel = device.ParentPanel;
			var parentUSB = device.ParentUSB;

			var result = new List<byte>();
			if (useId)
			{
				if (device.Driver.DriverType == DriverType.MS_1 || device.Driver.DriverType == DriverType.MS_2)
				{
					result.Add(0x01);
				}
				else
				{
					result.Add((byte)(parentPanel.Parent.IntAddress + 2));
					result.Add((byte)parentPanel.IntAddress);
				}
			}
			else
			{
				result.Add(0x02);
			}
			return result;
		}

		public static bool IsUsbDevice(Device device)
		{
			var usbHid = GetUsbUsbHid(device);
			if (usbHid != null)
			{
				return !usbHid.UseId;
			}
			return false;
		}

		static UsbHid GetUsbUsbHid(Device panelDevice)
		{
			var parentUSB = panelDevice.ParentUSB;
			if (parentUSB != null)
			{
				var usbHidInfo = UsbHidInfos.FirstOrDefault(x => x.USBDevice.UID == parentUSB.UID);
				if (usbHidInfo != null)
				{
					return usbHidInfo.UsbHid;
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
			UsbHidInfos = USBDetectorHelper.Detect();
			foreach (var usbHidInfo in UsbHidInfos)
			{
				usbHidInfo.UsbHid.DeviceRemoved += new Action<UsbHid>(UsbUsbHid_DeviceRemoved);
				usbHidInfo.UsbHid.NewResponse += new Action<Response>((response) =>
				{
					Trace.WriteLine("usbProcessorInfo.UsbProcessor.NewResponse " + usbHidInfo.UsbHid.No);
					if (usbHidInfo.USBDevice.Driver.DriverType == DriverType.MS_1)
					{
						Trace.WriteLine("DriverType.MS_1");
					}
					if (NewResponse != null)
						NewResponse(usbHidInfo.USBDevice, response);
				});
			}
		}

		public static event Action<Device, Response> NewResponse;

		static void UsbUsbHid_DeviceRemoved(UsbHid usbUsbHid)
		{
			var usbHidInfo = UsbHidInfos.FirstOrDefault(x => x.UsbHid == usbUsbHid);
			if (usbHidInfo != null)
			{
				UsbHidInfos.Remove(usbHidInfo);
			}
			if (UsbRemoved != null)
				UsbRemoved();
		}
		public static event Action UsbRemoved;

		public static void Dispose()
		{
			if (UsbHidInfos != null)
			{
				UsbHidInfos.ForEach(x => x.UsbHid.Dispose());
				UsbHidInfos.Clear();
			}
		}

		public static List<string> GetAllSerialNos()
		{
			Dispose();
			var result = new List<string>();
			var usbHidInfos = USBDetectorHelper.FindAllUsbHidInfo();
			foreach (var usbHidInfo in usbHidInfos)
			{
				result.Add(usbHidInfo.SerialNo);
				usbHidInfo.UsbHid.Dispose();
			}
			Thread.Sleep(TimeSpan.FromSeconds(5));
			return result;
		}
	}
}