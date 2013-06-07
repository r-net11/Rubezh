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
	}
}