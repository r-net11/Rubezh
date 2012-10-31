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
        static AutoResetEvent StopPingEvent;
        Thread PingThread;

        public void StopPingThread()
        {
            if (StopPingEvent != null)
            {
                StopPingEvent.Set();
            }
        }

        void StartPingThread()
        {
            if (PingThread == null)
            {
                StopPingEvent = new AutoResetEvent(false);
                PingThread = new Thread(new ThreadStart(() => { OnPing(); }));
                PingThread.Start();
            }
        }

        void OnPing()
        {
            while (true)
            {
                if (StopPingEvent.WaitOne(10000))
                    break;

                if (Connection != null)
                {
                    if (TasksCount == 0)
                    {
                        var result = GetCoreState();
                        if (result.HasError)
                        {
                            Logger.Error("NativeFiresecClient.OnPing");
                            DoConnect();
                        }
                    }
                }
            }
        }
    }
}