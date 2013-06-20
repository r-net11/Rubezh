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
		static AutoResetEvent CheckPauseEvent;
		static bool IsStopping = false;

		public static void StartMonitoring()
		{
			if (MonitoringThread == null)
			{
				Initialize();
				StartTime = DateTime.Now;
				MonitoringThread = new Thread(OnRun);
				MonitoringThread.Start();
			}
			StartTimeSynchronization();
		}

		public static void StopMonitoring()
		{
			IsStopping = true;
			SuspendMonitoring();
			ResumeMonitoring();

			if (MonitoringThread != null)
			{
				MonitoringThread.Join(TimeSpan.FromSeconds(2));
			}
			MonitoringThread = null;

			StopTimeSynchronization();
			IsStopping = false;
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

		public static bool CheckSuspending(bool throwException = true)
		{
			if (CheckPauseEvent != null)
				CheckPauseEvent.Set();

			if (PauseEvent != null)
			{
				if (PauseEvent.WaitOne(TimeSpan.FromMinutes(10)))
				{
					if (IsStopping)
					{
						if (throwException)
							throw new FS2StopMonitoringException();
						return true;
					}
				}
			}
			return false;
		}
	}
}