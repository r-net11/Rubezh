using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;

namespace Firesec
{
    public static class DispatcherFiresecClient
    {
        static Control control;

        delegate string StringDelegate();
        delegate string StringDelegateString(string arg1);
        delegate string StringDelegateIntInt(int arg1, int arg2);
        delegate string StringDelegateStringString(string arg1, string arg2);
        delegate string StringDelegateStringStringBool(string arg1, string arg2, bool arg3);
        delegate string StringDelegateStringStringString(string arg1, string arg2, string arg3);
        delegate bool BoolDelegateStringString(string arg1, string arg2);
        delegate bool BoolDelegateStringStringStringInt(string arg1, string arg2, string arg3, int arg4);

        static DispatcherFiresecClient()
        {
            control = new Control();

            var thread = new Thread(new ThreadStart(WorkTask));
            thread.Start();
        }

        public static bool Connect(string login, string password)
        {
            return (bool)control.Dispatcher.Invoke(new BoolDelegateStringString(NativeFiresecClient.Connect), login, password);
        }

        public static void Disconnect()
        {
            control.Dispatcher.Invoke(new Action(NativeFiresecClient.Disconnect));
        }

        public static string GetCoreConfig()
        {
            return control.Dispatcher.Invoke(new StringDelegate(NativeFiresecClient.GetCoreConfig)) as string;
        }

        public static string GetPlans()
        {
            return (string)control.Dispatcher.Invoke(new StringDelegate(NativeFiresecClient.GetPlans));
        }

        public static string GetMetaData()
        {
            return (string)control.Dispatcher.Invoke(new StringDelegate(NativeFiresecClient.GetMetadata));
        }

        public static string GetCoreState()
        {
            return (string)control.Dispatcher.Invoke(new StringDelegate(NativeFiresecClient.GetCoreState));
        }

        public static string GetCoreDeviceParams()
        {
            return control.Dispatcher.Invoke(new StringDelegate(NativeFiresecClient.GetCoreDeviceParams)) as string;
        }

        public static string ReadEvents(int fromId, int limit)
        {
            return control.Dispatcher.Invoke(new StringDelegateIntInt(NativeFiresecClient.ReadEvents), fromId, limit) as string;
        }

        public static void SetNewConfig(string coreConfig)
        {
            control.Dispatcher.Invoke(new Action<string>(NativeFiresecClient.SetNewConfig), coreConfig);
        }

        public static void DeviceWriteConfig(string coreConfig, string devicePath)
        {
            control.Dispatcher.Invoke(new Action<string, string>(NativeFiresecClient.DeviceWriteConfig), coreConfig, devicePath);
        }

        public static void ResetStates(string states)
        {
            control.Dispatcher.Invoke(new Action<string>(NativeFiresecClient.ResetStates), states);
        }

        public static void ExecuteCommand(string devicePath, string methodName)
        {
            control.Dispatcher.Invoke(new Action<string, string>(NativeFiresecClient.ExecuteCommand), devicePath, methodName);
        }

        public static void AddToIgnoreList(List<string> devicePaths)
        {
            control.Dispatcher.Invoke(new Action<List<string>>(NativeFiresecClient.AddToIgnoreList), devicePaths);
        }

        public static void RemoveFromIgnoreList(List<string> devicePaths)
        {
            control.Dispatcher.Invoke(new Action<List<string>>(NativeFiresecClient.RemoveFromIgnoreList), devicePaths);
        }

        public static void AddUserMessage(string message)
        {
            control.Dispatcher.Invoke(new Action<string>(NativeFiresecClient.AddUserMessage), message);
        }

        public static bool DeviceSetPassword(string coreConfig, string devicePath, string password, int deviceUser)
        {
            return (bool)control.Dispatcher.Invoke(new BoolDelegateStringStringStringInt(NativeFiresecClient.DeviceSetPassword), coreConfig, devicePath, password, deviceUser);
        }

        public static bool DeviceDatetimeSync(string coreConfig, string devicePath)
        {
            return (bool)control.Dispatcher.Invoke(new BoolDelegateStringString(NativeFiresecClient.DeviceDatetimeSync), coreConfig, devicePath);
        }

        public static string DeviceGetInformation(string coreConfig, string devicePath)
        {
            return (string)control.Dispatcher.Invoke(new StringDelegateStringString(NativeFiresecClient.DeviceGetInformation), coreConfig, devicePath);
        }

        public static string DeviceGetSerialList(string coreConfig, string devicePath)
        {
            return (string)control.Dispatcher.Invoke(new StringDelegateStringString(NativeFiresecClient.DeviceGetSerialList), coreConfig, devicePath);
        }

        public static string DeviceUpdateFirmware(string coreConfig, string devicePath, string fileName)
        {
            return (string)control.Dispatcher.Invoke(new StringDelegateStringStringString(NativeFiresecClient.DeviceUpdateFirmware), coreConfig, devicePath, fileName);
        }

        public static string DeviceVerifyFirmwareVersion(string coreConfig, string devicePath, string fileName)
        {
            return (string)control.Dispatcher.Invoke(new StringDelegateStringStringString(NativeFiresecClient.DeviceVerifyFirmwareVersion), coreConfig, devicePath, fileName);
        }

        public static string DeviceReadEventLog(string coreConfig, string devicePath)
        {
            return (string)control.Dispatcher.Invoke(new StringDelegateStringString(NativeFiresecClient.DeviceReadEventLog), coreConfig, devicePath);
        }

        public static string DeviceAutoDetectChildren(string coreConfig, string devicePath, bool fastSearch)
        {
            return (string)control.Dispatcher.Invoke(new StringDelegateStringStringBool(NativeFiresecClient.DeviceAutoDetectChildren), coreConfig, devicePath, fastSearch);
        }

        public static string DeviceCustomFunctionList(string driverUID)
        {
            return (string)control.Dispatcher.Invoke(new StringDelegateString(NativeFiresecClient.DeviceCustomFunctionList), driverUID);
        }

        public static string DeviceCustomFunctionExecute(string coreConfig, string devicePath, string functionName)
        {
            return (string)control.Dispatcher.Invoke(new StringDelegateStringStringString(NativeFiresecClient.DeviceCustomFunctionExecute), coreConfig, devicePath, functionName);
        }

        public static string DeviceGetGuardUsersList(string coreConfig, string devicePath)
        {
            return (string)control.Dispatcher.Invoke(new StringDelegateStringString(NativeFiresecClient.DeviceGetGuardUsersList), coreConfig, devicePath);
        }

        public static string DeviceSetGuardUsersList(string coreConfig, string devicePath, string users)
        {
            return (string)control.Dispatcher.Invoke(new StringDelegateStringStringString(NativeFiresecClient.DeviceSetGuardUsersList), coreConfig, devicePath, users);
        }

        public static string DeviceGetMDS5Data(string coreConfig, string devicePath)
        {
            return (string)control.Dispatcher.Invoke(new StringDelegateStringString(NativeFiresecClient.DeviceGetMDS5Data), coreConfig, devicePath);
        }

        public static string DeviceReadConfig(string coreConfig, string devicePath)
        {
            return (string)control.Dispatcher.Invoke(new StringDelegateStringString(NativeFiresecClient.DeviceReadConfig), coreConfig, devicePath);
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