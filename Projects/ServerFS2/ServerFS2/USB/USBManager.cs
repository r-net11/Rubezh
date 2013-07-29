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

		static object Locker = new object();
		public static List<UsbHidInfo> UsbHidInfos { get; set; }

		public static Response SendWithoutException(Device device, params object[] value)
		{
			return InternalSend(device, true, 15, value);
		}

		public static Response Send(Device device, params object[] value)
		{
			return InternalSend(device, false, 15, value);
		}

		public static Response SendShortAttempt(Device device, params object[] value)
		{
			return InternalSend(device, false, 1, value);
		}

		static Response InternalSend(Device device, bool throwException, int countRacall, params object[] value)
		{
			lock (Locker)
			{
				var usbHid = GetUsbUsbHid(device);
				if (usbHid != null)
				{
					var bytes = CreateBytesArray(value);
					var outputFunctionCode = Convert.ToByte(bytes[0]);
					var rootBytes = CreateRootBytes(device, usbHid.UseId);
					bytes.InsertRange(0, rootBytes);

					var response = usbHid.AddRequest(NextRequestNo, new List<List<byte>> { bytes }, 1000, 1000, true, countRacall);
					if (response != null)
					{
						response.InputBytes = response.Bytes.ToList();
						if (usbHid.UseId)
						{
							if (response.Bytes.Count < 4)
							{
								if (throwException)
									throw new FS2USBException("В ответе содержится недостаточное количество байт");
								return response.SetError("В ответе содержится недостаточное количество байт");
							}
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
					//Initialize();
					if (throwException)
						throw new FS2USBException("USB устройство отсутствует");
					return new Response("USB устройство отсутствует");
				}
			}
		}

		public static void SendAsync(Device device, Request request)
		{
			lock (Locker)
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
		}

		static List<byte> CreateRootBytes(Device device, bool useId)
		{
			var result = new List<byte>();
			if (useId)
			{
				if (device.Driver.DriverType == DriverType.MS_1 || device.Driver.DriverType == DriverType.MS_2)
				{
					result.Add(0x01);
				}
				else
				{
					var parentPanel = device.ParentPanel;
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
			if (panelDevice == null)
				return null;
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
			lock (Locker)
			{
				Dispose();
				UsbHidInfos = USBDetectorHelper.Detect();
				foreach (var usbHidInfo in UsbHidInfos)
				{
					usbHidInfo.UsbHid.DeviceRemoved += new Action<UsbHid>(UsbUsbHid_DeviceRemoved);
					usbHidInfo.UsbHid.NewResponse += new Action<UsbHidBase, Response>(OnNewResponse);
				}
			}
		}

		public static void ReInitialize(Device usbDevice)
		{
			lock (Locker)
			{
				var removingUsbHidInfo = UsbHidInfos.FirstOrDefault(x => x.USBDevice == usbDevice);
				if (removingUsbHidInfo != null)
				{
					removingUsbHidInfo.UsbHid.Dispose();
					UsbHidInfos.Remove(removingUsbHidInfo);
				}
				var newUsbHidInfos = USBDetectorHelper.Detect();
				foreach (var usbHidInfo in newUsbHidInfos)
				{
					usbHidInfo.UsbHid.DeviceRemoved += new Action<UsbHid>(UsbUsbHid_DeviceRemoved);
					usbHidInfo.UsbHid.NewResponse += new Action<UsbHidBase, Response>(OnNewResponse);
				}
				UsbHidInfos.AddRange(newUsbHidInfos);
			}
		}

		static void OnNewResponse(UsbHidBase usbHidBase, Response response)
		{
			try
			{
				var usbHidInfo = UsbHidInfos.FirstOrDefault(x => x.UsbHid != null && x.UsbHid == usbHidBase);
				if (usbHidInfo != null)
				{
					if (NewResponse != null)
						NewResponse(usbHidInfo.USBDevice, response);
				}
			}
			catch (Exception e)
			{
				;
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