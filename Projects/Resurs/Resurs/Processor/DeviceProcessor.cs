using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Resurs.Processor
{
	public class DeviceProcessor
	{
		AutoResetEvent StopEvent;
		Thread RunThread;

		public void Start()
		{
			if (RunThread == null)
			{
				StopEvent = new AutoResetEvent(false);
				RunThread = new Thread(OnRunThread);
				RunThread.Name = "Monitoring";
				RunThread.Start();
			}
		}

		public void Stop()
		{
			if (StopEvent != null)
			{
				StopEvent.Set();
			}
			if (RunThread != null)
			{
				RunThread.Join(TimeSpan.FromSeconds(5));
			}
			RunThread = null;
		}

		void OnRunThread()
		{
			while (true)
			{
				RunMonitoring();

				if (StopEvent != null)
				{
					if (StopEvent != null)
					{
						StopEvent.WaitOne(TimeSpan.FromMinutes(1));
					}
				}
			}
		}

		void RunMonitoring()
		{

		}
	}
}