using System;
using System.Collections.Generic;
using System.Threading;
using RubezhAPI;

namespace FiresecService.Service
{
	public class AsyncResult : IAsyncResult, IDisposable
	{
		AsyncCallback callback;
		object state;
		ManualResetEvent manualResentEvent;

		public AsyncResult(AsyncCallback callback, object state)
		{
			this.callback = callback;
			this.state = state;
			this.manualResentEvent = new ManualResetEvent(false);
		}

		object IAsyncResult.AsyncState
		{
			get { return state; }
		}

		public ManualResetEvent AsyncWait
		{
			get
			{
				return manualResentEvent;
			}
		}

		WaitHandle IAsyncResult.AsyncWaitHandle
		{
			get { return this.AsyncWait; }
		}

		bool IAsyncResult.CompletedSynchronously
		{
			get { return false; }
		}

		bool IAsyncResult.IsCompleted
		{
			get { return manualResentEvent.WaitOne(0, false); }
		}

		public void Complete()
		{
			manualResentEvent.Set();
			if (callback != null)
				callback(this);
		}

		public void Dispose()
		{
			manualResentEvent.Close();
			manualResentEvent = null;
			state = null;
			callback = null;
		}
	}

	public class PollAsyncResult : AsyncResult
	{
		public readonly int index = 0;
		public readonly DateTime dateTime = DateTime.Now;

		public Exception Exception { get; set; }
		private List<CallbackResult> callbackResult;
		public List<CallbackResult> Result
		{
			get { return callbackResult; }
			set { callbackResult = value; }
		}

		public PollAsyncResult(int index, DateTime dateTime, AsyncCallback asyncCallback, object state)
			: base(asyncCallback, state)
		{
			this.index = index;
			this.dateTime = dateTime;
		}
	}
}