using System;
using System.Collections.Generic;
using System.Linq;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using System.Threading;

namespace ServerFS2
{
	public class UsbRunner
	{
		UsbDevice _usbDevice;
		UsbEndpointReader _reader;
		UsbEndpointWriter _writer;
        private bool _stop = true;
        readonly Queue<Request> _requests = new Queue<Request>();
        List<byte> _result = new List<byte>();
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        private uint _requestId;
		public void Open()
		{
			var usbFinder = new UsbDeviceFinder(0xC251, 0x1303);
			_usbDevice = UsbDevice.OpenUsbDevice(usbFinder);
			if (_usbDevice == null)
				throw new Exception("Device Not Found.");
			var wholeUsbDevice = _usbDevice as IUsbDevice;
			if (!ReferenceEquals(wholeUsbDevice, null))
			{
				wholeUsbDevice.SetConfiguration(1);
				wholeUsbDevice.ClaimInterface(0);
			}
			_reader = _usbDevice.OpenEndpointReader(ReadEndpointID.Ep01);
			_reader.DataReceived += (OnDataRecieved);
			_reader.ReadBufferSize = 64;
			_reader.DataReceivedEnabled = true;
			_writer = _usbDevice.OpenEndpointWriter(WriteEndpointID.Ep01);
		}
		public void Close()
		{
			_reader.DataReceivedEnabled = false;
			_reader.DataReceived -= (OnDataRecieved);

			if (_usbDevice != null)
			{
				if (_usbDevice.IsOpen)
				{
					var wholeUsbDevice = _usbDevice as IUsbDevice;
					if (!ReferenceEquals(wholeUsbDevice, null))
					{
						wholeUsbDevice.ReleaseInterface(0);
					}
					_usbDevice.Close();
				}
				_usbDevice = null;
				UsbDevice.Exit();
			}
		}
        public void Send(List<byte> data)
		{
			int bytesWrite;
            _writer.Write(data.ToArray(), 2000, out bytesWrite);
		}
		void OnDataRecieved(object sender, EndpointDataEventArgs e)
		{
            var localresult = new List<byte>();
			foreach (var b in e.Buffer)
			{
				if (localresult.Count > 0)
				{
					localresult.Add(b);

					if (b == 0x3E)
					{
					    localresult = CreateInputBytes(localresult);
                        var request = _requests.FirstOrDefault(x => x.Id == _requestId);
                        if (request == null)
                            break;
					    var requestId = request.Id;
                        var responseId = (uint)(localresult.ToList()[3] + localresult.ToList()[2] * 256 + localresult.ToList()[1] * 256 * 256 + localresult.ToList()[0] * 256 * 256 * 256);
                        if (requestId == responseId)
                        {
                            _result  = localresult.ToList();
                            _autoResetEvent.Set();
                            _stop = true;
                        }
                        break;
					}
				}
				if (b == 0x7E)
				{

					localresult = new List<byte> {b};
				}
			}
		}
	    static List<byte> CreateOutputBytes(IEnumerable<byte> messageBytes)
		{
			var bytes = new List<byte>(0) {0x7e};
	        foreach (var b in messageBytes)
			{
				if (b == 0x7E)
				{
					bytes.Add(0x7D);
					bytes.Add(0x5E);
					continue;
				}
				if (b == 0x7D)
				{
					bytes.Add(0x7D);
					bytes.Add(0x5D);
					continue;
				}
				if (b == 0x3E)
				{
					bytes.Add(0x3D);
					bytes.Add(0x1E);
					continue;
				}
				if (b == 0x3D)
				{
					bytes.Add(0x3D);
					bytes.Add(0x1D);
					continue;
				}
				bytes.Add(b);
			}
			bytes.Add(0x3e);
			var bytesCount = bytes.Count;

			if (bytesCount < 64)
			{
				for (int i = 0; i < 64 - bytesCount; i++)
				{
					bytes.Add(0);
				}
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
                {
                    previousByte = b;
                    continue;
                }
                if (previousByte == 0x7D)
                {
                    if (b == 0x5E)
                    {
                        bytes.Add(0x7E);
                        continue;
                    }
                    if (b == 0x5D)
                    {
                        bytes.Add(0x7D);
                        continue;
                    }
                }
                if (previousByte == 0x3D)
                {
                    if (b == 0x1E)
                    {
                        bytes.Add(0x3E);
                        continue;
                    }
                    if (b == 0x1D)
                    {
                        bytes.Add(0x3D);
                        continue;
                    }
                }
                bytes.Add(b);
            }
            return bytes;
        }
        public OperationResult<Response> AddRequest(List<byte> data)
        {
            _stop = false;
            _requestId = (uint)(data[3] + data[2] * 256 + data[1] * 256 * 256 + data[0] * 256 * 256 * 256);
            data = CreateOutputBytes(data);
            var request = new Request
            {
                Id = _requestId,
                AutoResetEvent = new AutoResetEvent(false),
                Data = data
            };
            _requests.Enqueue(request);
            while (!_stop)
            {
                Send(data);
                _autoResetEvent.WaitOne(1000);
            }
            var response = new Response
            {
                Id = request.Id,
                Data = _result
            };
            return new OperationResult<Response> { Result = response };
        }
	}
    public class Request
    {
        public AutoResetEvent AutoResetEvent;
        public uint Id;
        public List<byte> Data;
    }
    public class Response
    {
        public uint Id;
        public List<byte> Data;
    }
}