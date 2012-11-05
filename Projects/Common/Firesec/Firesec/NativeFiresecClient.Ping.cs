using System;
using System.Threading;
using Common;

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
            if (PingThread != null)
            {
                PingThread.Join(TimeSpan.FromSeconds(1));
            }
        }

        void StartPingThread()
        {
            if (PingThread == null)
            {
                StopPingEvent = new AutoResetEvent(false);
                PingThread = new Thread(OnPing);
                PingThread.Start();
            }
        }

        void OnPing()
        {
            while (true)
            {
                if (StopPingEvent.WaitOne(10000))
                    break;

                if (!IsConnected)
                    continue;

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