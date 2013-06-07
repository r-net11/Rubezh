using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using FiresecAPI;

namespace ServerFS2
{
	public abstract class UsbRunnerBase
	{
		protected bool _stop = true;
		protected List<byte> _result = new List<byte>();
		protected readonly AutoResetEvent AutoResetEvent = new AutoResetEvent(false);
		protected readonly AutoResetEvent AautoWaitEvent = new AutoResetEvent(false);
		protected List<Response> Responses = new List<Response>();
		protected List<byte> LocalResult = new List<byte>();
		protected bool IsMs { get; set; }
		public static bool IsUsbDevice { get; set; }
		protected RequestCollection RequestCollection = new RequestCollection();

		public abstract bool Open();
		public abstract void Close();
		public abstract bool Send(List<byte> data);
		public abstract OperationResult<List<Response>> AddRequest(int usbRequestNo, List<List<byte>> bytesList, int delay, int timeout, bool isSyncronuos);

		public event Action<Response> NewResponse;
		protected void OnNewResponse(Response response)
		{
			if (NewResponse != null)
				NewResponse(response);
		}

		protected List<byte> CreateOutputBytes(IEnumerable<byte> messageBytes)
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

		protected List<byte> CreateInputBytes(List<byte> messageBytes)
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
	}
}