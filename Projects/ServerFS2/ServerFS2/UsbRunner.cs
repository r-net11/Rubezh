using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FiresecAPI;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace ServerFS2
{
	public class UsbRunner
	{
		UsbDevice _usbDevice;
		UsbEndpointReader _reader;
		UsbEndpointWriter _writer;
        private bool _stop = true;
        List<Request> _requests = new List<Request>();
        List<byte> _result = new List<byte>();
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _autoWaitEvent = new AutoResetEvent(false);
        private uint _requestId; 
		List<Response> _responses = new List<Response>();
		List<byte> _localresult = new List<byte>();
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

        private bool IsMs { get; set; }
		void OnDataRecieved(object sender, EndpointDataEventArgs e)
		{
            if (_isWrite)
            { 
                _autoResetEvent.Set();
                _requests.Clear();
                return;
            }
		    byte[] buffer;
            if (e.Buffer[0] == 0)
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
                buffer = e.Buffer.Where((val, idx) => (idx != 0) && (idx != 1)).ToArray();
		    else 
                buffer = e.Buffer.Where((val, idx) => (idx != 0)).ToArray();
		    foreach (var b in buffer)
			{
				if (_localresult.Count > 0)
				{
					_localresult.Add(b);
					if (b == 0x3E)
					{
						_localresult = CreateInputBytes(_localresult); // Преобразуем ответ в правильный вид
                        var responseId = (uint)(_localresult.ToList()[3] +
                            _localresult.ToList()[2] * 256 +
                            _localresult.ToList()[1] * 256 * 256 +
                            _localresult.ToList()[0] * 256 * 256 * 256); // id ответа
					    var request = _requests.FirstOrDefault();
						//var request = _requests.FirstOrDefault(x => x.Id == responseId); // среди всех запросов ищем запрос c id ответа
						//if (request == null) // если не нашли, то выходим из цикла, иначе
						//	break;
						_result = _localresult.ToList();
                        _localresult = new List<byte>();
						var response = new Response
										   {
											   Id = responseId,
											   Data = _result
										   };
                        OnNewResponse(response);
                        _responses.Clear();
						_responses.Add(response);
                        _requests.Clear();
					    //_requests.RemoveAll(x => x.Id == responseId);
						_autoResetEvent.Set();
						return;
					}
				}
				if (b == 0x7E)
				{
					_localresult = new List<byte> { b };
				}
                if (_requests.Count == 0)
                    _autoWaitEvent.Set();
			}
		}
		static List<byte> CreateOutputBytes(IEnumerable<byte> messageBytes)
		{
			var bytes = new List<byte>(0) { 0x7e };
			foreach (var b in messageBytes)
			{
				if (b == 0x7E)
                {bytes.Add(0x7D); bytes.Add(0x5E); continue;}
				if (b == 0x7D)
                {bytes.Add(0x7D); bytes.Add(0x5D); continue;}
				if (b == 0x3E)
				{bytes.Add(0x3D); bytes.Add(0x1E);continue;}
				if (b == 0x3D)
				{bytes.Add(0x3D); bytes.Add(0x1D);continue;}
				bytes.Add(b);
			}
			bytes.Add(0x3e);
			var bytesCount = bytes.Count;

			if (bytesCount < 64)
			{
				for (int i = 0; i < 64 - bytesCount; i++)
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
				{previousByte = b;continue;}
				if (previousByte == 0x7D)
				{
					if (b == 0x5E)
					{bytes.Add(0x7E);continue;}
					if (b == 0x5D)
					{bytes.Add(0x7D);continue;}
				}
				if (previousByte == 0x3D)
				{
					if (b == 0x1E)
					{bytes.Add(0x3E);continue;}
					if (b == 0x1D)
					{bytes.Add(0x3D);continue;}
				}
				bytes.Add(b);
			}
			return bytes;
		}
	    private bool _isWrite;
        // Если delay = 0, то запрос асинхронный, иначе синхронный с максимальным временем ожидания = delay
        public OperationResult<List<Response>> AddRequest(List<List<byte>> dataList, int delay, int timeout, bool IsWrite)
        {
            _isWrite = IsWrite;
            _responses = new List<Response>();
            _requests = new List<Request>();
            foreach (var dataOne in dataList)
            {
                var data = dataOne;
                _stop = false;
                //_requestId = (uint)(data[3] + data[2] * 256 + data[1] * 256 * 256 + data[0] * 256 * 256 * 256);
                data = CreateOutputBytes(data);
                // Создаем запрос
                var request = new Request
                {
                    //Id = _requestId,
                    Data = data
                };
                _requests.Add(request); // добавляем его в коллекцию всех запросов
                Send(data);
                _autoResetEvent.WaitOne(delay);
            }

            #region Для зашумленного канала (требует доработки, для многоблочных ответов)
            if (_requests.Count != 0) // Если у нас ещё остались не отвеченные запросы
            {
                var requests = new List<Request>(_requests);
                foreach (var request in requests)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        Send(request.Data);
                        _autoWaitEvent.WaitOne(timeout);
                        if (_requests.Count == 0)
                            break;
                    }
                }
            }
            #endregion
            return new OperationResult<List<Response>> { Result = _responses };
        }

        public void AddAsyncRequest(List<List<byte>> dataList, int delay, int timeout, bool IsWrite)
        {
            _isWrite = IsWrite;
            _responses = new List<Response>();
            _requests = new List<Request>();
            foreach (var dataOne in dataList)
            {
                var data = dataOne;
                _stop = false;
                _requestId = (uint)(data[3] + data[2] * 256 + data[1] * 256 * 256 + data[0] * 256 * 256 * 256);
                data = CreateOutputBytes(data);
                var request = new Request
                {
                    Id = _requestId,
                    Data = data
                };
                _requests.Add(request);
                Send(data);
                //_autoResetEvent.WaitOne(delay);
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