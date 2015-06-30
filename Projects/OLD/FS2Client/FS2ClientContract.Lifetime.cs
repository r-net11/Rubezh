﻿using System;
using System.Threading;
using Common;

namespace FS2Client
{
	public partial class FS2ClientContract
	{
		bool IsOperationByisy = false;
		DateTime StartOperationDateTime = DateTime.Now;
		DateTime CircleDateTime = DateTime.Now;
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
					if ((DateTime.Now - StartOperationDateTime) > TimeSpan.FromMinutes(15))
					{
						Logger.Error("FS2.OnRunLifetime StartOperationDateTime Expired");
						StopPollThread();
						StartPollThread();
					}
				}
				if ((DateTime.Now - CircleDateTime) > TimeSpan.FromMinutes(15))
				{
					Logger.Error("FS2.OnRunLifetime CircleDateTime Expired");
					StopPollThread();
					StartPollThread();
				}
			}
		}
	}
}