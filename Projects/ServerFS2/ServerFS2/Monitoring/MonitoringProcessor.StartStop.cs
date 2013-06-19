using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ServerFS2.Monitoring
{
	public static partial class MonitoringProcessor
	{
		static Thread MonitoringThread;
		static AutoResetEvent PauseEvent;
		static AutoResetEvent StopEvent;
		static AutoResetEvent CheckPauseEvent;
		static bool IsStopping;

		public static void StartMonitoring()
		{
			IsStopping = false;
			if (!DoMonitoring)
			{
				StartTime = DateTime.Now;
				DoMonitoring = true;
				MonitoringThread = new Thread(OnRun);
				MonitoringThread.Start();
			}
			StartTimeSynchronization();
		}

		public static void StopMonitoring()
		{
			if (IsStopping)
				return;

			IsStopping = true;
			DoMonitoring = false;
			SuspendMonitoring();
			ResumeMonitoring();
			LoopMonitoring = false;

			if (MonitoringThread != null)
			{
				MonitoringThread.Join(TimeSpan.FromSeconds(2));
			}
			MonitoringThread = null;

			StopTimeSynchronization();
		}

		public static bool SuspendMonitoring()
		{
			if (PauseEvent != null)
				PauseEvent.Set();
			PauseEvent = new AutoResetEvent(false);

			CheckPauseEvent = new AutoResetEvent(false);
			var result = CheckPauseEvent.WaitOne(TimeSpan.FromSeconds(10));
			CheckPauseEvent = null;
			return result;
		}

		public static void ResumeMonitoring()
		{
			if (PauseEvent != null)
				PauseEvent.Set();
			PauseEvent = null;
		}

		public static void CheckSuspending()
		{
			if (CheckPauseEvent != null)
				CheckPauseEvent.Set();

			if (PauseEvent != null)
			{
				if (PauseEvent.WaitOne(TimeSpan.FromMinutes(10)))
				{
					if (IsStopping)
						throw new FS2StopMonitoringException();
				}
			}
		}
	}
}