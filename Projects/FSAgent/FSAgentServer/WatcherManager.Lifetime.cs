using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Common;

namespace FSAgentServer
{
    public partial class WatcherManager
    {
        AutoResetEvent StopLifetimeEvent;
        Thread LifetimeThread;
		bool IsOperationBuisy;
		DateTime OperationDateTime = DateTime.Now;
		DateTime CircleDateTime = DateTime.Now;

        void StartLifetimeThread()
        {
            if (LifetimeThread == null)
            {
                StopLifetimeEvent = new AutoResetEvent(false);
                LifetimeThread = new Thread(OnLifetime);
                LifetimeThread.Start();
            }
        }

        void StopLifetimeThread()
        {
            if (StopLifetimeEvent != null)
            {
                StopLifetimeEvent.Set();
            }
            if (LifetimeThread != null)
            {
                LifetimeThread.Join(TimeSpan.FromSeconds(1));
            }
        }

        void OnLifetime()
        {
            while (true)
            {
                if (IsOperationBuisy)
                {
                    if (DateTime.Now - OperationDateTime > TimeSpan.FromMinutes(15))
                    {
                        Logger.Error("WatcherManager.WatchLifetime");
                        App.Restart();
                    }
                }
				if (DateTime.Now - CircleDateTime > TimeSpan.FromMinutes(15))
				{
					Logger.Error("WatcherManager.WatchLifetime");
					App.Restart();
				}

                if (StopLifetimeEvent.WaitOne(10000))
                    break;
            }
        }
    }
}