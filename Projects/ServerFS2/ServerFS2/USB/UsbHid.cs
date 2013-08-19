using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UsbLibrary;
using ServerFS2.ViewModels;

namespace ServerFS2
{
	public partial class UsbHid
	{
		public UsbHidPort UsbHidPort { get; private set; }
		bool IsDisposed = false;

		public bool Open()
		{
			UsbHidPort = new UsbHidPort()
			{
				VendorId = 0xC251,
				ProductId = 0x1303
			};
			UsbHidPort.CheckDevicePresent();
			if (UsbHidPort.SpecifiedDevice == null)
				return false;
			UsbHidPort.SpecifiedDevice.DataRecieved += new DataRecievedEventHandler(OnDataRecieved);
			UsbHidPort.SpecifiedDevice.OnDeviceRemoved += new EventHandler(UsbHidPort_OnDeviceRemoved);
			return true;
		}

		public void SetUsbHidPort(UsbHidPort usbHidPort)
		{
			UsbHidPort = usbHidPort;
			UsbHidPort.SpecifiedDevice.DataRecieved += new DataRecievedEventHandler(OnDataRecieved);
			UsbHidPort.SpecifiedDevice.OnDeviceRemoved += new EventHandler(UsbHidPort_OnDeviceRemoved);
		}

		void UsbHidPort_OnDeviceRemoved(object sender, EventArgs e)
		{
			if (DeviceRemoved != null)
				DeviceRemoved(this);
		}
		public event Action<UsbHid> DeviceRemoved;

		public void Dispose()
		{
			IsDisposed = true;
			AutoWaitEvent.Set();
			UsbHidPort.SpecifiedDevice.DataRecieved -= new DataRecievedEventHandler(OnDataRecieved);
			UsbHidPort.SpecifiedDevice.Dispose();
			UsbHidPort.Dispose();
			//AutoWaitEvent.Set();
		}

		public bool Send(List<byte> bytes, string name, int attemptNo)
		{
			//Trace.WriteLine(DateTime.Now.TimeOfDay.ToString() + " - Send Name = " + name + " - " + attemptNo);
			var fs2LogItem = new FS2LogItem()
			{
				DateTime = DateTime.Now,
				Name = name,
				AttemptNo = attemptNo
			};
			LogService.AddUSBHidLog(fs2LogItem);
			if (IsDisposed)
				return false;
			UsbHidPort.SpecifiedDevice.SendData(bytes.ToArray());
			return true;
		}

		void OnDataRecieved(object sender, DataRecievedEventArgs args)
		{
			if (IsDisposed)
				return;

			var buffer = args.data.ToList();
			if (buffer.Count < 2)
				return;
			buffer.RemoveRange(0, IsExtendedMode ? 3 : 2);
			foreach (var b in buffer)
			{
				if (LocalResult.Count > 0)
				{
					LocalResult.Add(b);
					if (b == 0x3E)
					{
						var bytes = CreateInputBytes(LocalResult);
						LocalResult = new List<byte>();

						var response = new Response
						{
							Bytes = bytes.ToList()
						};
						if (UseId)
						{
							response.Id = BytesHelper.ExtractInt(bytes.ToList(), 0);
						}
						OnResponseRecieved(response);
						OnNewResponse(response);
						return;
					}
				}
				if (b == 0x7E)
				{
					if (!IsExtendedMode)
					{
						if (buffer.IndexOf(0x7e) == 0)
							IsExtendedMode = false;
						if (buffer.IndexOf(0x7e) == 1)
							IsExtendedMode = true;
					}
					LocalResult = new List<byte> { b };
				}
				if (RequestCollection.Count() == 0)
					AutoWaitEvent.Set();
			}
		}

		void OnResponseRecieved(Response response)
		{
			if (UseId)
			{
				var request = RequestCollection.GetById(response.Id);
				RequestCollection.RemoveById(response.Id);
				if (request != null)
				{
					Responses.Add(response);
				}
			}
			else
			{
				var request = RequestCollection.GetFirst();
				if (request != null)
				{
					Responses.Clear();
					Responses.Add(response);
					if (Responses.Count != 0)
						RequestCollection.Clear();
				}
			}
		}

		public Response AddRequest(int usbRequestNo, List<List<byte>> bytesList, int delay, int timeout, bool isSyncronuos, int countRacall, string name)
		{
			Responses = new List<Response>();
			RequestCollection.Clear();
			foreach (var bytes in bytesList)
			{
				if (usbRequestNo != -1)
				{
					if (bytesList.Count > 1)
					{
						usbRequestNo++;
					}
				}
				var request = new Request();
				if (UseId)
				{
					request.Id = usbRequestNo;
					if (usbRequestNo != -1)
					{
						bytes.InsertRange(0, BitConverter.GetBytes(usbRequestNo).Reverse());
					}
				}
				request.Bytes = CreateOutputBytes(bytes);
				RequestCollection.AddRequest(request);
				if (request.Bytes.Count > 64)
				{
					AutoWaitEvent = new AutoResetEvent(false);
					for (int i = 0; i < request.Bytes.Count / 64; i++)
					{
						var bytesToSend = request.Bytes.GetRange(i * 64, 64);
						bytesToSend.Insert(0, 0);
						Send(bytesToSend, name, -1);
					}
					AutoWaitEvent.WaitOne();
					RequestCollection.Clear();
					return Responses.FirstOrDefault();
				}
				else
				{
					request.Bytes.Insert(0, 0);
					if (isSyncronuos)
					{
						AutoWaitEvent = new AutoResetEvent(false);
					}
					Send(request.Bytes, name, -1);
					if (isSyncronuos)
					{
						AutoWaitEvent.WaitOne(delay);
					}
				}
			}

			if (isSyncronuos)
			{
				foreach (var request in new List<Request>(RequestCollection.Requests))
				{
					if (request != null)
					{
						for (int i = 0; i < countRacall; i++)
						{
							if (IsDisposed)
							{
								RequestCollection.Clear();
								return null;
							}

							if (RequestCollection.Count() == 0)
								break;

							AutoWaitEvent.Reset();

							var result = Send(request.Bytes, name, i);
							if (!result)
							{
								RequestCollection.Clear();
								return null;
							}

							if (RequestCollection.Count() == 0)
								break;
							AutoWaitEvent.WaitOne(timeout);

							if (i > 5)
							{
								Trace.WriteLine("CountRacall = " + i + " - " + name);
							}
						}
					}
				}
				RequestCollection.Clear();
				return Responses.FirstOrDefault();
			}
			RequestCollection.Clear();
			return null;
		}
	}
}