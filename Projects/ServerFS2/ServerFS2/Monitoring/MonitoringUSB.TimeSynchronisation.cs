using System;
using System.Threading;

namespace ServerFS2.Monitoring
{
	public partial class MonitoringUSB
	{
		Thread TimeSynchronizationThread;
		AutoResetEvent TimeSynchronizationEvent;
		bool IsTimeSynchronizationNeeded = false;

		public void StartTimeSynchronization()
		{
			TimeSynchronizationEvent = new AutoResetEvent(false);
			if (TimeSynchronizationThread == null)
			{
				TimeSynchronizationThread = new Thread(OnTimeSynchronization);
				TimeSynchronizationThread.Start();
			}
		}

		public void StopTimeSynchronization()
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

		void OnTimeSynchronization()
		{
			while (!TimeSynchronizationEvent.WaitOne(TimeSpan.FromDays(1)))
			{
				IsTimeSynchronizationNeeded = true;
			}
		}
	}
}