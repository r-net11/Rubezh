using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ServerFS2.Monitoring
{
	public static partial class MonitoringProcessor
	{
		static Thread TimeSynchronizationThread;
		static AutoResetEvent TimeSynchronizationEvent;
		static bool IsTimeSynchronizationNeeded = false;

		public static void StartTimeSynchronization()
		{
			TimeSynchronizationEvent = new AutoResetEvent(false);
			if (TimeSynchronizationThread == null)
			{
				TimeSynchronizationThread = new Thread(OnTimeSynchronization);
				TimeSynchronizationThread.Start();
			}
		}

		public static void StopTimeSynchronization()
		{
			if (TimeSynchronizationThread != null)
			{
				if (TimeSynchronizationEvent != null)
				{
					TimeSynchronizationEvent.Set();
				}
				TimeSynchronizationThread.Join(TimeSpan.FromSeconds(1));
			}
			TimeSynchronizationThread = null;
		}

		static void OnTimeSynchronization()
		{
			while (!TimeSynchronizationEvent.WaitOne(TimeSpan.FromDays(1)))
			{
				IsTimeSynchronizationNeeded = true;
			}
		}
	}
}