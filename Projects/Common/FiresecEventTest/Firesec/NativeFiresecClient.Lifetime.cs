using System;
using System.Threading;
using Common;

namespace Firesec
{
    public partial class NativeFiresecClient
    {
        static AutoResetEvent StopLifetimeEvent;
        Thread LifetimeThread;

        void StartLifetimeThread()
        {
            if (LifetimeThread == null)
            {
                StopLifetimeEvent = new AutoResetEvent(false);
                LifetimeThread = new Thread(OnLifetime);
                LifetimeThread.Start();
            }
        }

        public void StopLifetimeThread()
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
                if (IsConnected && (IsOperationBuisy || TasksCount > 0))
                {
                    if (DateTime.Now - OperationDateTime > TimeSpan.FromMinutes(10))
                    {
                        Logger.Error("NativeFiresecClient.WatchLifetime");
                        DoConnect();
                    }
                }

                if (StopLifetimeEvent.WaitOne(10000))
                    break;
            }
        }
    }
}