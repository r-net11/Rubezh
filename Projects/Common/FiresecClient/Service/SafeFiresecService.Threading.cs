using System;
using System.Collections.Generic;
using System.Threading;
using Common;

namespace FiresecClient
{
	public partial class SafeFiresecService
	{
		public static int TasksCount;
		Queue<Action> Tasks = new Queue<Action>();
		object locker = new object();
		Thread OperationQueueThread;
		bool IsStopping;
		AutoResetEvent SuspendOperationQueueEvent;

		public void StartOperationQueueThread()
		{
			IsStopping = false;
			if (OperationQueueThread == null)
			{
				OperationQueueThread = new Thread(OnOperationQueue);
				OperationQueueThread.Name = "SafeFiresecService OperationQueue";
				OperationQueueThread.IsBackground = true;
				OperationQueueThread.Start();
			}
		}

		public void StopOperationQueueThread()
		{
			IsStopping = true;
			if (OperationQueueThread != null)
			{
				OperationQueueThread.Join(TimeSpan.FromSeconds(2));
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
				Logger.Error(e, "SafeFiresecService.AddTask");
			}
		}

		void OnOperationQueue()
		{
			while (true)
			{
				try
				{
					lock (locker)
					{
						if (IsStopping)
							return;

						if (Tasks == null)
						{
							Tasks = new Queue<Action>();
							Logger.Error("SafeFiresecService.Work Tasks = null");
						}

						while (Tasks.Count == 0)
						{
							if (IsStopping)
								return;
							Monitor.Wait(locker, TimeSpan.FromSeconds(1));
						}

						if (SuspendPoll)
						{
							Thread.Sleep(TimeSpan.FromSeconds(1));
							continue;
						}
					}

					if (SuspendOperationQueueEvent != null)
					{
						SuspendOperationQueueEvent.WaitOne(TimeSpan.FromMinutes(1));
					}

					var action = Tasks.Dequeue();
					if (action != null)
					{
						//Dispatcher.CurrentDispatcher.Invoke(action);
						action();
						TasksCount = Tasks.Count;
					}
					else
					{
						Tasks = new Queue<Action>();
						Logger.Error("SafeFiresecService.Work action = null");
					}
				}
				catch (Exception e)
				{
					Logger.Error(e, "SafeFiresecService.Work");
				}
			}
		}
	}
}