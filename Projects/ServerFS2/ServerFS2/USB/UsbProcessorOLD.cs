using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using FiresecAPI;
using FS2Api;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace ServerFS2
{
	public class UsbProcessorOLD : UsbProcessorBase
	{
		UsbDevice UsbDevice;
		UsbEndpointReader Reader;
		UsbEndpointWriter Writer;

		public override bool Open()
		{
			var usbFinder = new UsbDeviceFinder(0xC251, 0x1303);
			UsbDevice = UsbDevice.OpenUsbDevice(usbFinder);
			if (UsbDevice == null)
			{
				throw new Exception("Device Not Found.");
			}

			var wholeUsbDevice = UsbDevice as IUsbDevice;
			if (wholeUsbDevice != null)
			{
				wholeUsbDevice.SetConfiguration(1);
				wholeUsbDevice.ClaimInterface(0);
			}
			Reader = UsbDevice.OpenEndpointReader(ReadEndpointID.Ep01);
			Reader.DataReceived += (OnDataRecieved);
			Reader.ReadBufferSize = 64;
			Reader.DataReceivedEnabled = true;
			Writer = UsbDevice.OpenEndpointWriter(WriteEndpointID.Ep01);
			return true;
		}

		public override void Dispose()
		{
			if (Reader != null)
			{
				Reader.DataReceivedEnabled = false;
				Reader.DataReceived -= (OnDataRecieved);
			}

			if (UsbDevice != null)
			{
				if (UsbDevice.IsOpen)
				{
					var wholeUsbDevice = UsbDevice as IUsbDevice;
					if (!ReferenceEquals(wholeUsbDevice, null))
					{
						wholeUsbDevice.ReleaseInterface(0);
					}
					UsbDevice.Close();
				}
				UsbDevice = null;
				UsbDevice.Exit();
			}
		}

		public override bool Send(List<byte> data)
		{
			Trace.WriteLine(BytesHelper.BytesToString(data));
			int bytesWrite;
			if (Writer == null)
				throw new FS2Exception("Драйвер USB отсутствует");
			Writer.Write(data.ToArray(), 2000, out bytesWrite);
			return bytesWrite == data.Count;
		}

		void OnDataRecieved(object sender, EndpointDataEventArgs e)
		{
			var buffer = e.Buffer.ToList();
			if (buffer.Count < 2)
				return;
		    buffer.RemoveRange(0, IsExtendedMode ? 2 : 1);
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
					AutoWaitEvent.Set();
			}
		}

		void OnResponseRecieved(Response response)
		{
			Request request;
			if (WithoutId)
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

		public override Response AddRequest(int usbRequestNo, List<List<byte>> bytesList, int delay, int timeout, bool isSyncronuos, int countRacall = 15)
		{
			Responses = new List<Response>();
			RequestCollection.Clear();
			foreach (var bytes in bytesList)
			{
				if (bytesList.Count > 1)
				{
					usbRequestNo++;
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
						Send(request.Bytes.GetRange(i * 64, 64));
				}
				else
				{
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
						Send(request.Bytes);
						AutoWaitEvent.WaitOne(timeout);
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