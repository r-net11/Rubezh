using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FiresecAPI;
using UsbLibrary;
using System.Diagnostics;

namespace ServerFS2
{
	public class UsbProcessor : UsbProcessorBase
	{
		UsbHidPort UsbHidPort;

		public override bool Open()
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

		void UsbHidPort_OnDeviceRemoved(object sender, EventArgs e)
		{
			OnDeviceRemoved(this);
		}

		public event Action<UsbProcessor> DeviceRemoved;
		protected void OnDeviceRemoved(UsbProcessor usbProcessor)
		{
			if (DeviceRemoved != null)
				DeviceRemoved(usbProcessor);
		}

		public override void Dispose()
		{
			UsbHidPort.SpecifiedDevice.DataRecieved -= new DataRecievedEventHandler(OnDataRecieved);
			UsbHidPort.SpecifiedDevice.Dispose();
			UsbHidPort.Dispose();
		}

		public override bool Send(List<byte> bytes)
		{
			//Trace.WriteLine("Send " + BytesHelper.BytesToString(bytes));
			UsbHidPort.SpecifiedDevice.SendData(bytes.ToArray());
			return true;
		}

		void OnDataRecieved(object sender, DataRecievedEventArgs args)
		{
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
						if (!WithoutId)
						{
							response.Id = BytesHelper.ExtractInt(bytes.ToList(), 0);
						}
						OnResponseRecieved(response);
						AutoResetEvent.Set();
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
					AautoWaitEvent.Set();
			}
		}

		void OnResponseRecieved(Response response)
		{
			//Trace.WriteLine("response " + BytesHelper.BytesToString(response.Bytes));
			if (WithoutId)
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
			else
			{
				var request = RequestCollection.GetById(response.Id);
				RequestCollection.RemoveById(response.Id);
				if (request != null)
				{
					Responses.Add(response);
				}
			}
		}

		public override Response AddRequest(int usbRequestNo, List<List<byte>> bytesList, int delay, int timeout, bool isSyncronuos, int countRacall = 15)
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
				if (!WithoutId)
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
					for (int i = 0; i < request.Bytes.Count / 64; i++)
					{
						var bytesToSend = request.Bytes.GetRange(i * 64, 64);
						bytesToSend.Insert(0, 0);
						Send(bytesToSend);
					}
				}
				else
				{
					request.Bytes.Insert(0, 0);
					Send(request.Bytes);
				}
				if (isSyncronuos)
				{
					AutoResetEvent.WaitOne(delay);
				}
			}

			if (isSyncronuos)
			{
				foreach (var request in new List<Request>(RequestCollection.Requests))
				{
					for (int i = 0; i < countRacall; i++)
					{
						Send(request.Bytes);
						AautoWaitEvent.WaitOne(timeout);
						if (RequestCollection.Count() == 0)
							break;
					}
				}
				return Responses.FirstOrDefault();
			}
			return null;
		}
	}
}