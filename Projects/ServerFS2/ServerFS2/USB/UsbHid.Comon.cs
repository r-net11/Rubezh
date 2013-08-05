using System;
using System.Collections.Generic;
using System.Threading;
using FiresecAPI;

namespace ServerFS2
{
	public partial class UsbHid
	{
		AutoResetEvent AutoWaitEvent = new AutoResetEvent(false);
		List<Response> Responses = new List<Response>();
		List<byte> LocalResult = new List<byte>();
		public bool UseId { get; set; }
		bool IsExtendedMode { get; set; }
		RequestCollection RequestCollection = new RequestCollection();

		public event Action<UsbHid, Response> NewResponse;
		void OnNewResponse(Response response)
		{
			if (NewResponse != null)
				NewResponse(this, response);
		}

		List<byte> CreateOutputBytes(IEnumerable<byte> messageBytes)
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

		List<byte> CreateInputBytes(List<byte> messageBytes)
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