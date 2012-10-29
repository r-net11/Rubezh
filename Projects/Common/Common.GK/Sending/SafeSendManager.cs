using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using System.Threading;

namespace Common.GK
{
    public class SafeSendManager
    {
        public SafeSendManager()
        {
            StartThread();
        }

        public void Send(SendPriority sendPriority, XDevice device, ushort length, byte command, ushort inputLenght, List<byte> data = null, bool hasAnswer = true, bool sleepInsteadOfRecieve = false)
        {
            SendManager.Send(device, length, command, inputLenght, data, hasAnswer, sleepInsteadOfRecieve);
        }

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
                Logger.Error(e, "SafeSendManager.AddTask");
            }
        }

        void Work()
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
                            Logger.Error("SafeSendManager.Work Tasks = null");
                        }

                        while (Tasks.Count == 0)
                            Monitor.Wait(locker, TimeSpan.FromSeconds(1));
                    }

                    if (IsSuspending)
                    {
                        Thread.Sleep(500);
                        continue;
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
                        Logger.Error("SafeSendManager.Work action = null");
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e, "SafeSendManager.Work");
                }
            }
        }
    }

    public enum SendPriority
    {
        Low,
        Normal,
        Heigh
    }
}