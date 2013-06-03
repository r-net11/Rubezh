using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace USBTest
{
	public class Test
	{
		Task task;
		CancellationTokenSource cancellationTokenSource;
		CancellationToken cancellationToken;

		public void Start()
		{
			cancellationTokenSource = new CancellationTokenSource();
			cancellationToken = cancellationTokenSource.Token;
			task = Task.Factory.StartNew(OnTask, cancellationToken);
			cancellationToken.Register(OnCancelled);
		}

		public void Stop()
		{
			Trace.WriteLine("task.Status " + task.Status.ToString());
			cancellationTokenSource.Cancel();
			Trace.WriteLine("task.Status " + task.Status.ToString());
			try
			{
				task.Wait(2000);
			}
			catch (AggregateException ex)
			{
				Trace.WriteLine(ex.Message);
			}
			var exception = task.Exception;
			task.Dispose();
			cancellationTokenSource.Dispose();
		}

		public void Stop2()
		{
			task.Dispose();
		}

		void OnTask()
		{
			int index = 0;
			while (true)
			{
				cancellationToken.ThrowIfCancellationRequested();
				//if (cancellationToken.IsCancellationRequested)
				//{
				//    //return;
				//    throw new OperationCanceledException();
				//}
				Trace.WriteLine("OnTask " + index++.ToString());
				cancellationToken.WaitHandle.WaitOne(1000);
				//Thread.Sleep(1000);
			}
		}

		void OnCancelled()
		{
			Trace.WriteLine("OnCancelled");
		}
	}
}
