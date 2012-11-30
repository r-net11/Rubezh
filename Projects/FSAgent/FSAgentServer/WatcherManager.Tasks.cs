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
		Queue<Action> NonBlockingTasks = new Queue<Action>();
		Queue<BlockingTask> BlockingTasks = new Queue<BlockingTask>();
		object locker = new object();

		public void AddNonBlockingTask(Action task)
		{
			try
			{
				lock (locker)
				{
					NonBlockingTasks.Enqueue(task);
					Monitor.Pulse(locker);
					PoolSleepEvent.Set();
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "FSAgentContract.AddTask");
			}
		}

		public object AddBlockingTask(Func<object> func)
		{
			var blockingTask = new BlockingTask(func);
			BlockingTasks.Enqueue(blockingTask);
			blockingTask.ReadyEvent.WaitOne(TimeSpan.FromMinutes(8));
			PoolSleepEvent.Set();
			return blockingTask.Result;
		}

		void CheckNonBlockingTasks()
		{
			try
			{
				while (NonBlockingTasks.Count > 0)
				{
					var action = NonBlockingTasks.Dequeue();
					if (action != null)
					{
						OperationDateTime = DateTime.Now;
						IsOperationBuisy = true;
						try
						{
							action();
						}
						catch (Exception ex)
						{
							Logger.Error(ex, "WatcherManager.CheckNonBlockingTasks.Action");
						}
						finally
						{
							IsOperationBuisy = false;
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "WatcherManager.CheckNonBlockingTasks");
			}
		}

		void CheckBlockingTasks()
		{
			try
			{
				while (BlockingTasks.Count > 0)
				{
					var dispatcherItem = BlockingTasks.Dequeue();
					if (dispatcherItem != null)
					{
						OperationDateTime = DateTime.Now;
						IsOperationBuisy = true;
						try
						{
							dispatcherItem.Execute();
						}
						catch (Exception ex)
						{
							Logger.Error(ex, "WatcherManager.CheckBlockingTasks.Action");
						}
						finally
						{
							IsOperationBuisy = false;
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "WatcherManager.CheckNonBlockingTasks");
			}
		}
	}

	public class BlockingTask
	{
		public Func<object> Method { get; set; }
		public object Result { get; set; }
		public AutoResetEvent ReadyEvent { get; set; }

		public BlockingTask(Func<object> method)
		{
			Method = method;
			ReadyEvent = new AutoResetEvent(false);
		}

		public void Execute()
		{
			Result = Method();
			ReadyEvent.Set();
		}
	}
}