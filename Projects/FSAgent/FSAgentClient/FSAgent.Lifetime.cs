using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using FiresecAPI.Models;
using FSAgentAPI;
using System.Diagnostics;
using Common;

namespace FSAgentClient
{
	public partial class FSAgent
	{
		bool IsOperationByisy = false;
		DateTime StartOperationDateTime;
		Thread LifetimeThread;
		AutoResetEvent LifetimeEvent;

		public void StartLifetime()
		{
			LifetimeThread = new Thread(OnRunLifetime);
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
				LifetimeEvent = new AutoResetEvent(false);
				if (LifetimeEvent.WaitOne(TimeSpan.FromMinutes(1)))
					return;

				if (IsOperationByisy)
				{
					if ((DateTime.Now - StartOperationDateTime) > TimeSpan.FromMinutes(10))
					{
						Logger.Error("FSAgent.OnRunLifetime Time Expired");
					}
				}
			}
		}
	}
}