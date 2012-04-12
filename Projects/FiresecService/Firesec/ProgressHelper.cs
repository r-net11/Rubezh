using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Firesec
{
    public static class ProgressHelper
    {
        static object locker = new object();
        static Queue<ProgressData> taskQeue = new Queue<ProgressData>();
        public static event Func<int, string, int, int, bool> Progress;

        static ProgressHelper()
        {
            var thread = new Thread(new ThreadStart(WorkTask));
            thread.Start();
        }

        public static void ProcessProgress(int Stage, string Comment, int PercentComplete, int BytesRW)
        {
            lock (locker)
            {
                var progressData = new ProgressData()
                {
                    Stage = Stage,
                    Comment = Comment,
                    PercentComplete = PercentComplete,
                    BytesRW = BytesRW
                };

                taskQeue.Enqueue(progressData);
                Monitor.PulseAll(locker);
            }
        }

        static void WorkTask()
        {
            while (true)
            {
                lock (locker)
                {
                    while (taskQeue.Count == 0)
                        Monitor.Wait(locker);

                    ProgressData progressData = taskQeue.Dequeue();
                    var result = OnProgress(progressData.Stage, progressData.Comment, progressData.PercentComplete, progressData.BytesRW);

                    Interlocked.Exchange(ref NotificationCallBack.IntContinueProgress, result ? 1 : 0);

                    Trace.WriteLine("Task: " + progressData.Comment);
                }
            }
        }

        static bool OnProgress(int Stage, string Comment, int PercentComplete, int BytesRW)
        {
            if (Progress != null)
                return Progress(Stage, Comment, PercentComplete, BytesRW);
            return true;
        }
    }
}