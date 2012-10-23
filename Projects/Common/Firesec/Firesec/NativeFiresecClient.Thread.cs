using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Firesec
{
    public partial class NativeFiresecClient
    {
		public static int TasksCount;
        Queue<Action> Tasks = new Queue<Action>();
        object locker = new object();
        Thread WorkThread;
        bool IsStopping;
		bool IsSuspending = false;

        public void StartThread()
        {
            IsStopping = false;
            WorkThread = new Thread(Work);
            WorkThread.IsBackground = true;
            WorkThread.Start();
        }

        public void StopThread()
        {
            IsStopping = true;
            WorkThread.Join(TimeSpan.FromSeconds(2));
        }

        public void AddTask(Action task)
        {
            lock (locker)
            {
                Tasks.Enqueue(task);
                Monitor.Pulse(locker);
            }
			Trace.WriteLine("Tasks Count = " + TasksCount.ToString());
        }

        void Work()
        {
            while (true)
            {
                lock (locker)
                {
                    if (IsStopping)
                        return;

                    while (Tasks.Count == 0)
                        Monitor.Wait(locker, TimeSpan.FromSeconds(1));
                }

				if (IsSuspending)
				{
					Thread.Sleep(500);
					continue;
				}

                var action = Tasks.Dequeue();
                action();
				TasksCount = Tasks.Count;
            }
        }
    }
}