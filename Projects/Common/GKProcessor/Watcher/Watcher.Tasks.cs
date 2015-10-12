﻿using System;
using System.Collections.Generic;
using System.Threading;
using Common;

namespace GKProcessor
{
	public partial class Watcher
	{
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
				Logger.Error(e, "Watcher.AddTask");
			}
		}

		void CheckTasks()
		{
			try
			{
				var actions = new List<Action>();
				lock (locker)
				{
					if (Tasks == null)
					{
						Tasks = new Queue<Action>();
						Logger.Error("Watcher.CheckTasks Tasks = null");
					}

					while (Tasks.Count > 0)
					{
						var action = Tasks.Dequeue();
						if (action != null)
						{
							actions.Add(action);
						}
					}
				}
				foreach (var action in actions)
				{
					action();
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Watcher.CheckTasks");
			}
		}
	}
}