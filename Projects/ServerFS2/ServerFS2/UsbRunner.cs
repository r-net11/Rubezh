using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using FiresecAPI;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace ServerFS2
{
	public class UsbRunner
	{
		UsbDevice UsbDevice;
		UsbEndpointReader Reader;
		UsbEndpointWriter Writer;
		private bool _stop = true;
		List<Request> Requests = new List<Request>();
		List<byte> _result = new List<byte>();
		private readonly AutoResetEvent AutoResetEvent = new AutoResetEvent(false);
		private readonly AutoResetEvent AautoWaitEvent = new AutoResetEvent(false);
		List<Response> Responses = new List<Response>();
		List<byte> Localresult = new List<byte>();
		private bool IsMs { get; set; }
		public static bool IsUsbDevice { get; set; }

		public bool Open()
		{
			var usbFinder = new UsbDeviceFinder(0xC251, 0x1303);
			UsbDevice = UsbDevice.OpenUsbDevice(usbFinder);
			if (UsbDevice == null)
			{
				throw new Exception("Device Not Found.");
				return false;
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

		public void Close()
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

		public bool Send(List<byte> data)
		{
			int bytesWrite;
			Writer.Write(data.ToArray(), 2000, out bytesWrite);
			return bytesWrite == data.Count;
		}

		void OnDataRecieved(object sender, EndpointDataEventArgs e)
		{
			var buffer = e.Buffer.ToList();
			if (buffer.Count < 2)
				return;

			if (buffer[0] == 0)
			{
				IsMs = false;
				ServerHelper.IsExtendedMode = false; // если не МС, то выключаем расширенный режим
			}
			else
			{
				if (!IsMs) // Если МС, (а до этого был не МС, то посылаем запрос, на проверку расширенного режима)
					ServerHelper.IsExtendedModeMethod();
				IsMs = true;
			}
			if (ServerHelper.IsExtendedMode)
			{
				buffer.RemoveRange(0, 2);
			}
			else
			{
				buffer.RemoveRange(0, 1);
			}

			foreach (var b in buffer)
			{
				if (Localresult.Count > 0)
				{
					Localresult.Add(b);
					if (b == 0x3E)
					{
						var bytes = CreateInputBytes(Localresult);
						Localresult = new List<byte>();

						var response = new Response()
						{
							Data = bytes.ToList()
						};
						Request request = null;
						if (IsUsbDevice)
						{
							request = Requests.FirstOrDefault();
							if (request != null)
							{
								Responses.Clear();
								Responses.Add(response);
								if (Responses.Count != 0)
									Requests.Clear();
							}
						}
						else
						{
							var responseId = (uint)(bytes.ToList()[3] +
									bytes.ToList()[2] * 256 +
									bytes.ToList()[1] * 256 * 256 +
									bytes.ToList()[0] * 256 * 256 * 256);
							request = Requests.FirstOrDefault(x => x.Id == responseId);
							response.Id = responseId;
							Requests.RemoveAll(x => x.Id == responseId);
							if (request != null)
							{
								Responses.Add(response);
							}
						}

						AutoResetEvent.Set();
						return;
					}
				}
				if (b == 0x7E)
				{
					Localresult = new List<byte> { b };
				}
				if (Requests.Count == 0)
					AautoWaitEvent.Set();
			}
		}

		static List<byte> CreateOutputBytes(IEnumerable<byte> messageBytes)
		{
		    var bytes = new List<byte>(0) { 0x7e };
			foreach (var b in messageBytes)
			{
				if (b == 0x7E)
				{ bytes.Add(0x7D); bytes.Add(0x5E); continue; }
				if (b == 0x7D)
				{ bytes.Add(0x7D); bytes.Add(0x5D); continue; }
				if (b == 0x3E)
				{ bytes.Add(0x3D); bytes.Add(0x1E); continue; }
				if (b == 0x3D)
				{ bytes.Add(0x3D); bytes.Add(0x1D); continue; }
				bytes.Add(b);
			}
            bytes.Add(0x3e);

            while (bytes.Count % 64 > 0)
			{
                bytes.Add(0);
			}
			return bytes;
		}

		static List<byte> CreateInputBytes(List<byte> messageBytes)
		{
			var bytes = new List<byte>();
			var previousByte = new byte();
			messageBytes.RemoveRange(0, messageBytes.IndexOf(0x7E) + 1);
			messageBytes.RemoveRange(messageBytes.IndexOf(0x3E), messageBytes.Count - messageBytes.IndexOf(0x3E));
			foreach (var b in messageBytes)
			{
				if ((b == 0x7D) || (b == 0x3D))
				{ previousByte = b; continue; }
				if (previousByte == 0x7D)
				{
                    previousByte = new byte();
					if (b == 0x5E)
					{ bytes.Add(0x7E); continue; }
					if (b == 0x5D)
					{ bytes.Add(0x7D); continue; }
				}
				if (previousByte == 0x3D)
				{
                    previousByte = new byte();
					if (b == 0x1E)
					{ bytes.Add(0x3E); continue; }
					if (b == 0x1D)
					{ bytes.Add(0x3D); continue; }
				}
				bytes.Add(b);
			}
			return bytes;
		}

		public OperationResult<List<Response>> AddRequest(List<List<byte>> bytesList, int delay, int timeout, bool isSyncronuos)
		{
			Responses = new List<Response>();
			Requests = new List<Request>();
			foreach (var bytes in bytesList)
			{
				_stop = false;
				var request = new Request();
				if (!IsUsbDevice)
				{
					request.Id = (uint)(bytes[3] + bytes[2] * 256 + bytes[1] * 256 * 256 + bytes[0] * 256 * 256 * 256); ;
				}
				request.Data = CreateOutputBytes(bytes);
				Requests.Add(request);
				if (request.Data.Count > 64)
				{
					for (int i = 0; i < request.Data.Count / 64; i++)
						Send(request.Data.GetRange(i * 64, 64));
				}
				else
				{
					Send(request.Data);
				}
				if (isSyncronuos)
				{
					AutoResetEvent.WaitOne(delay);
				}
			}

			if (isSyncronuos)
			{
				foreach (var request in new List<Request>(Requests))
				{
					for (int i = 0; i < 15; i++)
					{
						Send(request.Data);
						AautoWaitEvent.WaitOne(timeout);
						if (Requests.Count == 0)
							break;
					}
				}
				return new OperationResult<List<Response>> { Result = Responses };
			}
			else
			{
				return null;
			}
		}

		public event Action<Response> NewResponse;
		void OnNewResponse(Response response)
		{
			if (NewResponse != null)
				NewResponse(response);
		}
	}

	public class Request
	{
		public uint Id;
		public List<byte> Data;
	}

	public class Response
	{
		public uint Id;
		public List<byte> Data;
	}
}