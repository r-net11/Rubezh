using System;
using System.Threading;
using Common;

namespace FSAgentClient
{
	public partial class FSAgent
	{
		bool IsOperationByisy = false;
		DateTime StartOperationDateTime = DateTime.Now;
		DateTime CircleDateTime = DateTime.Now;
		Thread LifetimeThread;
		ManualResetEvent LifetimeEvent;

		public void StartLifetime()
		{
			LifetimeEvent = new ManualResetEvent(false);
			LifetimeThread = new Thread(OnRunLifetime);
			LifetimeThread.Name = "FSAgent Lifetime";
			LifetimeThread.Start();
		}

		public void StopLifetime()
		{
			if (LifetimeEvent != null)
			{
				LifetimeEvent.Set();
				LifetimeThread.Join(TimeSpan.FromSeconds(5));
			}
		}

		void OnRunLifetime()
		{
			while (true)
			{
				if (LifetimeEvent.WaitOne(TimeSpan.FromMinutes(1)))
					return;

				if (IsOperationByisy)
				{
					if ((DateTime.Now - StartOperationDateTime) > TimeSpan.FromMinutes(15))
					{
						Logger.Error("FSAgent.OnRunLifetime StartOperationDateTime Expired");
						StopPollThread();
						StartPollThread();
					}
				}
				if ((DateTime.Now - CircleDateTime) > TimeSpan.FromMinutes(15))
				{
					Logger.Error("FSAgent.OnRunLifetime CircleDateTime Expired");
					StopPollThread();
					StartPollThread();
				}
			}
		}
	}
}