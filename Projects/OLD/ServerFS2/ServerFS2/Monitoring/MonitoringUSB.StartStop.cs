using System;
using System.Threading;

namespace ServerFS2.Monitoring
{
	public partial class MonitoringUSB
	{
		Thread MonitoringThread;
		AutoResetEvent PauseEvent;
		AutoResetEvent CheckPauseEvent;
		bool IsStopping = false;
		bool IsSuspended = false;

		public void StartMonitoring()
		{
			if (MonitoringThread == null)
			{
				StartTime = DateTime.Now;
				MonitoringThread = new Thread(OnRun);
				MonitoringThread.Start();
			}
			StartTimeSynchronization();
		}

		public void StopMonitoring()
		{
			IsStopping = true;
			SuspendMonitoring();
			PauseEvent = null;

			MonitoringPanels.Clear();
			MonitoringNonPanels.Clear();

			if (MonitoringThread != null)
			{
				MonitoringThread.Join(TimeSpan.FromSeconds(2));
			}
			MonitoringThread = null;

			StopTimeSynchronization();
			IsStopping = false;
		}

		public void SuspendMonitoring()
		{
			if (PauseEvent != null)
				PauseEvent.Set();
			PauseEvent = new AutoResetEvent(false);

			if (!IsSuspended)
			{
				CheckPauseEvent = new AutoResetEvent(false);
				var result = CheckPauseEvent.WaitOne(TimeSpan.FromSeconds(10));
				CheckPauseEvent = null;
			}
			IsSuspended = true;

			SetAllInitializing(false);
		}

		public void ResumeMonitoring()
		{
			if (PauseEvent != null)
				PauseEvent.Set();
			PauseEvent = null;
			RemoveAllInitializing(false);
			IsSuspended = false;
		}

		public bool CheckSuspending(bool throwException = true)
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