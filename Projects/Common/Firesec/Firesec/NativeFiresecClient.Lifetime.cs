using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using Common;
using FiresecAPI;
using System.Diagnostics;

namespace Firesec
{
    public partial class NativeFiresecClient
    {
        static AutoResetEvent StopLifetimeEvent;
        Thread LifetimeThread;

        public void StopLifetimeThread()
        {
            if (StopLifetimeEvent != null)
            {
                StopLifetimeEvent.Set();
            }
        }

        void StartLifetimeThread()
        {
            if (PingThread == null)
            {
                StopLifetimeEvent = new AutoResetEvent(false);
                PingThread = new Thread(new ThreadStart(() => { OnLifetime(); }));
                PingThread.Start();
            }
        }

        void OnLifetime()
        {
            while (true)
            {
                if (IsOperationBuisy)
                {
                    if (DateTime.Now - OperationDateTime > TimeSpan.FromMinutes(10))
                    {
                        Logger.Error("NativeFiresecClient.WatchLifetime");
                        SocketServerHelper.Restart();
                        DoConnect();
                    }
                }

                if (StopLifetimeEvent.WaitOne(10000))
                    break;
            }
        }
    }
}