using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using System.Text;

namespace Firesec
{
    public static class DispatcherFiresecClient
    {
        static Control control;

        static DispatcherFiresecClient()
        {
            control = new Control();

            var thread = new Thread(new ThreadStart(WorkTask));
            thread.Start();
        }

        public static T SafeExecute<T>(Func<T> func)
        {
            return (T)control.Dispatcher.Invoke(func);
        }

        public static FiresecOperationResult<T> ConvertResultData<T>(FiresecOperationResult<string> result)
        {
            var resultData = new FiresecOperationResult<T>();
            resultData.HasError = result.HasError;
            resultData.Error = result.Error;
            if (result.HasError == false)
                resultData.Result = Deserialize<T>(result.Result);
            return resultData;
        }

        static T Deserialize<T>(string input)
        {
            if (string.IsNullOrEmpty(input))
                return default(T);

            try
            {
                using (var memoryStream = new MemoryStream(Encoding.Default.GetBytes(input)))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(memoryStream);
                }
            }
            catch (Exception e)
            {
                return default(T);
            }
        }

        public static void ProcessProgress(int Stage, string Comment, int PercentComplete, int BytesRW)
        {
            var progressData = new ProgressData()
            {
                Stage = Stage,
                Comment = Comment,
                PercentComplete = PercentComplete,
                BytesRW = BytesRW
            };
            AddTask(progressData);

            //var backgroundWorker = new BackgroundWorker();
            //backgroundWorker.DoWork += delegate(object s, DoWorkEventArgs args)
            //{
            //    OnProgress(Stage, Comment, PercentComplete, BytesRW);
            //};
            //backgroundWorker.RunWorkerAsync();

            //Control xControl = new Control();
            //control.Dispatcher.Invoke(new FiresecEventAggregator.ProgressDelegate(OnProgress), Stage, Comment, PercentComplete, BytesRW);

            //OnProgress(Stage, Comment, PercentComplete, BytesRW);
            //control.Dispatcher.BeginInvoke(new FiresecEventAggregator.ProgressDelegate(OnProgress), Stage, Comment, PercentComplete, BytesRW);
        }

        static object locker = new object();
        static Queue<ProgressData> taskQ = new Queue<ProgressData>();

        static void AddTask(ProgressData progressData)
        {
            lock (locker)
            {
                taskQ.Enqueue(progressData);
                Monitor.PulseAll(locker);
            }
        }

        static void WorkTask()
        {
            while (true)
            {
                ProgressData progressData;

                lock (locker)
                {
                    while (taskQ.Count == 0)
                        Monitor.Wait(locker);

                    progressData = taskQ.Dequeue();

                    var result = OnProgress(progressData.Stage, progressData.Comment, progressData.PercentComplete, progressData.BytesRW);
                    NotificationCallBack.ContinueProgress = result;
                }

                Trace.WriteLine("Task: " + progressData.Comment);
            }
        }

        static bool OnProgress(int Stage, string Comment, int PercentComplete, int BytesRW)
        {
            if (Progress != null)
                return Progress(Stage, Comment, PercentComplete, BytesRW);
            return true;
        }

        public static event FiresecEventAggregator.ProgressDelegate Progress;
    }

    public class ProgressData
    {
        public int Stage { get; set; }
        public string Comment { get; set; }
        public int PercentComplete { get; set; }
        public int BytesRW { get; set; }
    }
}