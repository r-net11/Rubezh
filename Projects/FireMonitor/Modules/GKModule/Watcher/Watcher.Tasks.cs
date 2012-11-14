using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.GK;
using FiresecAPI.XModels;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using XFiresecAPI;
using System.Threading;

namespace GKModule
{
	public partial class Watcher
	{
		public static int TasksCount;
		Queue<Action> Tasks = new Queue<Action>();
		object locker = new object();

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
				Logger.Error(e, "NativeFiresecClient.AddTask");
			}
		}

		void CheckTasks()
		{
			try
			{
				lock (locker)
				{
					if (Tasks == null)
					{
						Tasks = new Queue<Action>();
						Logger.Error("JournalWatcher.CheckTasks Tasks = null");
					}

					if (Tasks.Count == 0)
						return;
				}

				var action = Tasks.Dequeue();
				if (action != null)
				{
					action();
					TasksCount = Tasks.Count;
				}
				else
				{
					Tasks = new Queue<Action>();
					Logger.Error("JournalWatcher.CheckTasks action = null");
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "JournalWatcher.CheckTasks");
			}
		}
	}
}