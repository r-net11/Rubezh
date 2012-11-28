using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Infrastructure.Common;

namespace FSAgentServer
{
    public static class Bootstrapper
    {
        static WatcherManager WatcherManager;

        public static void Run()
        {
            FSAgentServiceHost.Start();
			FSAgentLoadHelper.NotifyStartCompleted();
            WatcherManager = new WatcherManager();
            WatcherManager.Start();
        }
    }
}