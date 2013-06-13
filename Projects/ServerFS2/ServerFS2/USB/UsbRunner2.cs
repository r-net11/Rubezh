using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using FiresecAPI;
using System.Diagnostics;
using ServerFS2.ConfigurationWriter;

namespace ServerFS2
{
	public class UsbRunner2 : UsbRunnerBase
	{
		UsbLibrary.UsbHidPort UsbHidPort;

		public override bool Open()
		{
			UsbHidPort = new UsbLibrary.UsbHidPort();
			UsbHidPort.VendorId = 0xC251;
			UsbHidPort.ProductId = 0x1303;
			UsbHidPort.CheckDevicePresent();
			UsbHidPort.SpecifiedDevice.DataRecieved += new UsbLibrary.DataRecievedEventHandler(OnDataRecieved);
			return true;
		}

		public override void Close()
		{
			UsbHidPort.Dispose();
		}

		public override bool Send(List<byte> data)
		{
			data.Insert(0, 0);
			UsbHidPort.SpecifiedDevice.SendData(data.ToArray());
			return true;
		}

		void OnDataRecieved(object sender, UsbLibrary.DataRecievedEventArgs args)
		{
			var buffer = args.data.ToList();
			if (buffer.Count < 2)
				return;
			IsMs = buffer[0] != 0;
			buffer.RemoveRange(0, ServerHelper.IsExtendedMode ? 3 : 2);
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
							Data = bytes.ToList()
						};
						if (!IsUsbDevice)
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
					if (!ServerHelper.IsExtendedMode)
					{
						if (buffer.IndexOf(0x7e) == 0)
							ServerHelper.IsExtendedMode = false;
						if (buffer.IndexOf(0x7e) == 1)
							ServerHelper.IsExtendedMode = true;
					}
					LocalResult = new List<byte> { b };
				}
				if (RequestCollection.Count() == 0)
					AautoWaitEvent.Set();
			}
		}

		void OnResponseRecieved(Response response)
		{
			Request request;
			if (IsUsbDevice)
			{
				request = RequestCollection.GetFirst();
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
				//Trace.WriteLine("response.Id = " + response.Id);
				request = RequestCollection.GetById(response.Id);
				RequestCollection.RemoveById(response.Id);
				if (request != null)
				{
					Responses.Add(response);
				}
			}
		}

		public override OperationResult<List<Response>> AddRequest(int usbRequestNo, List<List<byte>> bytesList, int delay, int timeout, bool isSyncronuos)
		{
			Responses = new List<Response>();
			RequestCollection.Clear();
			foreach (var bytes in bytesList)
			{
				if (bytesList.Count > 1)
				{
					usbRequestNo++;
				}
				_stop = false;
				var request = new Request();
				if (!IsUsbDevice)
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
						Send(request.Bytes.GetRange(i * 64, 64));
				}
				else
				{
					//Trace.WriteLine("request.Id = " + request.Id);
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
					for (int i = 0; i < 15; i++)
					{
						//Trace.WriteLine("request.Id = " + request.Id);
						Send(request.Bytes);
						AautoWaitEvent.WaitOne(timeout);
						if (RequestCollection.Count() == 0)
							break;
					}
				}
				return new OperationResult<List<Response>> { Result = Responses };
			}
			return null;
		}
	}
}