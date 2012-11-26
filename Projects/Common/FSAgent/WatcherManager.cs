using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firesec;
using FiresecAPI;
using System.Threading;
using System.Diagnostics;
using FiresecAPI.Models;
using System.Windows.Threading;
using Common;

namespace FSAgent
{
    public partial class WatcherManager
	{
        AutoResetEvent StopEvent;
        Thread RunThread;
        FiresecSerializedClient FiresecSerializedClient;
        FiresecSerializedClient CallbackFiresecSerializedClient;
        Watcher Watcher;
        int PollIndex = 0;
        bool IsOperationBuisy;
        DateTime OperationDateTime;

        public void Start()
        {
            if (RunThread == null)
            {

                CallbackFiresecSerializedClient = new FiresecSerializedClient();
                CallbackFiresecSerializedClient.Connect("localhost", 211, "adm", "", true);

                StopEvent = new AutoResetEvent(false);
                RunThread = new Thread(OnRun);
                RunThread.SetApartmentState(ApartmentState.STA);
                RunThread.IsBackground = true;
                RunThread.Start();

                StartLifetimeThread();
            }
        }

        public void Stop()
        {
            StopLifetimeThread();

            if (StopEvent != null)
            {
                StopEvent.Set();
            }
            if (RunThread != null)
            {
                RunThread.Join(TimeSpan.FromSeconds(1));
            }
        }

		void OnRun()
		{
			try
			{
                FiresecSerializedClient = new FiresecSerializedClient();
                FiresecSerializedClient.Connect("localhost", 211, "adm", "", false);

                Watcher = new Watcher(FiresecSerializedClient, true, true);
			}
			catch (Exception e)
			{
				Logger.Error(e, "OnRun");
			}

			while (true)
			{
				try
				{
					Thread.Sleep(TimeSpan.FromSeconds(1));
                    PollIndex++;
                    var force = PollIndex % 100 == 0;

                    OperationDateTime = DateTime.Now;
                    IsOperationBuisy = true;
                    try
                    {
                        FiresecSerializedClient.NativeFiresecClient.CheckForRead(force);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, "OnRun.while");
                    }
                    finally
                    {
                        IsOperationBuisy = false;
                    }
				}
				catch (Exception e)
				{
					Logger.Error(e, "OnRun.while2");
				}
			}
		}
	}
}