using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DiagnosticsModule.ViewModels
{
    public class AsyncTest
    {
        Queue<Action> Tasks;
        object locker = new object();
        Thread WorkThread;
        bool IsStopping;

        public AsyncTest()
        {
            Tasks = new Queue<Action>();
        }

        public void Start()
        {
            IsStopping = false;
            WorkThread = new Thread(Work);
            WorkThread.IsBackground = true;
            WorkThread.Start();
        }

        public void Stop()
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

                var action = Tasks.Dequeue();
                action();
            }
        }
    }
}