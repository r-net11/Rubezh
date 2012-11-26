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

namespace FSAgentServer
{
    public partial class WatcherManager
	{
		public static WatcherManager Current { get; private set; }
        AutoResetEvent StopEvent;
        Thread RunThread;
		public FiresecSerializedClient FiresecSerializedClient { get; private set; }
        FiresecSerializedClient CallbackFiresecSerializedClient;
        Watcher Watcher;
        int PollIndex = 0;
        bool IsOperationBuisy;
        DateTime OperationDateTime;

		public WatcherManager()
		{
			Current = this;
		}

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

				var testThread = new Thread(OnTest);
				//testThread.Start();
            }
        }

		void OnTest()
		{
			while (true)
			{
				Thread.Sleep(TimeSpan.FromSeconds(5));
				AddTask(new Action(() =>
				{
					FiresecSerializedClient.NativeFiresecClient.AddUserMessage("UserMessage");
				}));
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
						while (Tasks.Count > 0)
						{
							var action = Tasks.Dequeue();
							if (action != null)
								action();
						}

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

		public void AddTask(Action task)
		{
			try
			{
				lock (locker)
				{
					Tasks.Enqueue(task);
					Monitor.Pulse(locker);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "FSAgentContract.AddTask");
			}
		}

		Queue<Action> Tasks = new Queue<Action>();
		object locker = new object();
	}
}