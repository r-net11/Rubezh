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
using FSAgentAPI;

namespace FSAgentServer
{
    public partial class WatcherManager
	{
		public static WatcherManager Current { get; private set; }
        AutoResetEvent StopEvent = new AutoResetEvent(false);
        Thread RunThread;
		public NativeFiresecClient AdministratorClient { get; private set; }
		public NativeFiresecClient MonitorClient { get; private set; }
		NativeFiresecClient CallbackClient;
        Watcher Watcher;
        int PollIndex = 0;
        bool IsOperationBuisy;
        DateTime OperationDateTime;
		public static AutoResetEvent PoolSleepEvent = new AutoResetEvent(false);

		public WatcherManager()
		{
			Current = this;
		}

        public void Start()
        {
            if (RunThread == null)
            {
				CallbackClient = new NativeFiresecClient();
                CallbackClient.Connect("localhost", 211, "adm", "", true);

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

		public static void WaikeOnEvent()
		{
			PoolSleepEvent.Set();
		}

		void OnRun()
		{
			try
			{
				AdministratorClient = new NativeFiresecClient();
				AdministratorClient.Connect("localhost", 211, "adm", "", false);
				AdministratorClient.ProgressEvent += new Func<int, string, int, int, bool>(OnAdministratorProgress);

				MonitorClient = new NativeFiresecClient();
                MonitorClient.Connect("localhost", 211, "adm", "", false);

                Watcher = new Watcher(MonitorClient, true, true);
			}
			catch (Exception e)
			{
				Logger.Error(e, "OnRun");
			}

			while (true)
			{
				try
				{
					PoolSleepEvent = new AutoResetEvent(false);
					PoolSleepEvent.WaitOne(TimeSpan.FromSeconds(1));
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

                        while (DelegateTasks.Count > 0)
                        {
                            var dispatcherItem = DelegateTasks.Dequeue();
                            dispatcherItem.Execute();
                        }

                        MonitorClient.CheckForRead(force);
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
					PoolSleepEvent.Set();
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "FSAgentContract.AddTask");
			}
		}

		Queue<Action> Tasks = new Queue<Action>();
		object locker = new object();

        public object Invoke(Func<object> func)
        {
            var dispatcherItem = new DispatcherItem(func);
            DelegateTasks.Enqueue(dispatcherItem);
			PoolSleepEvent.Set();
            dispatcherItem.FuncInvokeEvent.WaitOne(TimeSpan.FromSeconds(100));
            return dispatcherItem.Result;
        }

        Queue<DispatcherItem> DelegateTasks = new Queue<DispatcherItem>();

		bool OnAdministratorProgress(int stage, string comment, int percentComplete, int bytesRW)
		{
			Trace.WriteLine("comment = " + comment);
			LastFSProgressInfo = new FSProgressInfo()
			{
				Stage = stage,
				Comment = comment,
				PercentComplete = percentComplete,
				BytesRW = bytesRW
			};
			ClientsManager.ClientInfos.ForEach(x => x.PollWaitEvent.Set());
			return true;
		}

		public FSProgressInfo LastFSProgressInfo { get; set; }
	}

    public class DispatcherItem
    {
        public Func<object> Method { get; set; }
        public object Result { get; set; }
        public AutoResetEvent FuncInvokeEvent { get; set; }

        public DispatcherItem(Func<object> method)
        {
            Method = method;
            FuncInvokeEvent = new AutoResetEvent(false);
        }

        public void Execute()
        {
            Result = Method();
            FuncInvokeEvent.Set();
        }
    }
}