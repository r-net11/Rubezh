using FiresecAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FiresecService
{
	public static class ServerTaskRunner
	{
		static List<ServerOperationTask> ServerOperationTasks = new List<ServerOperationTask>();

		static Thread Thread;
		static AutoResetEvent AutoResetEvent = new AutoResetEvent(false);
		public static void Start()
		{
			Thread = new Thread(OnRun);
			Thread.Start();
		}

		public static void Stop()
		{
			foreach (var serverOperationTask in ServerOperationTasks)
			{
				if(serverOperationTask.ProgressCallback != null)
				{
					serverOperationTask.ProgressCallback.IsCanceled = true;
				}
			}

			if(AutoResetEvent != null)
			{
				AutoResetEvent.Set();
				if(Thread != null)
				{
					Thread.Join(TimeSpan.FromSeconds(2));
				}
			}
		}

		public static void SetNewConfig()
		{
			Stop();
			Start();
		}

		static void OnRun()
		{
			AutoResetEvent = new AutoResetEvent(false);
			while(true)
			{
				if(AutoResetEvent.WaitOne(TimeSpan.FromSeconds(1)))
				{
					return;
				}
				var serverOperationTask = ServerOperationTasks.FirstOrDefault();
				if(serverOperationTask != null)
				{
					serverOperationTask.Action();
					ServerOperationTasks.RemoveAt(0);
				}
			}
		}

		public static void Add(GKProgressCallback progressCallback, Action action)
		{
			var serverOperationTask = new ServerOperationTask() { Action = action, ProgressCallback = progressCallback };
			ServerOperationTasks.Add(serverOperationTask);
		}
	}

	public class ServerOperationTask
	{
		public Action Action { get; set; }
		public GKProgressCallback ProgressCallback { get; set; }
	}
}