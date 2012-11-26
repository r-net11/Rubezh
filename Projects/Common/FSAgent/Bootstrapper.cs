using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSAgent.Service;
using System.Threading;

namespace FSAgent
{
    public static class Bootstrapper
    {
        static WatcherManager WatcherManager;

        public static void Run()
        {
            ConfigurationManager.GetConfiruration();
            FSAgentServiceHost.Start();
            WatcherManager = new WatcherManager();
            WatcherManager.Start();

            return;
            var thread = new Thread(OnRun);
            thread.Start();
        }

        static void OnRun()
        {
            FSAgentServiceHost.Start();
        }
    }
}