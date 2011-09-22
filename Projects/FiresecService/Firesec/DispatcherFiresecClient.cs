using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Firesec
{
    public static class DispatcherFiresecClient
    {
        static Control control;

        delegate string StringDelegate();
        delegate string StringDelegateIntInt(int arg1, int arg2);
        delegate bool BoolDelegateStringString(string arg1, string arg2);
        delegate string StringDelegateStringString(string arg1, string arg2);
        delegate string StringDelegateStringStringBool(string arg1, string arg2, bool arg3);
        delegate string StringDelegateStringStringString(string arg1, string arg2, string arg3);

        static DispatcherFiresecClient()
        {
            control = new Control();
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

        public static void DeviceSetPassword(string coreConfig, string devicePath, string password, int deviceUser)
        {
            control.Dispatcher.Invoke(new Action<string, string, string, int>(NativeFiresecClient.DeviceSetPassword), coreConfig, devicePath, password, deviceUser);
        }

        public static void DeviceDatetimeSync(string coreConfig, string devicePath)
        {
            control.Dispatcher.Invoke(new Action<string, string>(NativeFiresecClient.DeviceDatetimeSync), coreConfig, devicePath);
        }

        public static void DeviceRestart(string coreConfig, string devicePath)
        {
            control.Dispatcher.Invoke(new Action<string, string>(NativeFiresecClient.DeviceRestart), coreConfig, devicePath);
        }

        public static string DeviceGetInformation(string coreConfig, string devicePath)
        {
            return (string)control.Dispatcher.Invoke(new StringDelegateStringString(NativeFiresecClient.DeviceGetInformation), coreConfig, devicePath);
        }

        public static string DeviceGetSerialList(string coreConfig, string devicePath)
        {
            return (string)control.Dispatcher.Invoke(new StringDelegateStringString(NativeFiresecClient.DeviceGetSerialList), coreConfig, devicePath);
        }

        public static string DeviceUpdateFirmware(string coreConfig, string devicePath, string content)
        {
            return (string)control.Dispatcher.Invoke(new StringDelegateStringStringString(NativeFiresecClient.DeviceUpdateFirmware), coreConfig, devicePath, content);
        }

        public static string DeviceVerifyFirmwareVersion(string coreConfig, string devicePath, string content)
        {
            return (string)control.Dispatcher.Invoke(new StringDelegateStringStringString(NativeFiresecClient.DeviceVerifyFirmwareVersion), coreConfig, devicePath, content);
        }

        public static string DeviceReadEventLog(string coreConfig, string devicePath)
        {
            return (string)control.Dispatcher.Invoke(new StringDelegateStringString(NativeFiresecClient.DeviceReadEventLog), coreConfig, devicePath);
        }

        public static string DeviceAutoDetectChildren(string coreConfig, string devicePath, bool fastSearch)
        {
            return (string)control.Dispatcher.Invoke(new StringDelegateStringStringBool(NativeFiresecClient.DeviceAutoDetectChildren), coreConfig, devicePath, fastSearch);
        }

        public static string DeviceReadConfig(string coreConfig, string devicePath)
        {
            return (string)control.Dispatcher.Invoke(new StringDelegateStringString(NativeFiresecClient.DeviceReadConfig), coreConfig, devicePath);
        }

        public static bool ProcessProgress(int Stage, string Comment, int PercentComplete, int BytesRW)
        {
            return (bool)control.Dispatcher.Invoke(new FiresecEventAggregator.ProgressDelegate(OnProgress), Stage, Comment, PercentComplete, BytesRW);
        }

        static bool OnProgress(int Stage, string Comment, int PercentComplete, int BytesRW)
        {
            if (Progress != null)
                return Progress(Stage, Comment, PercentComplete, BytesRW);
            return false;
        }

        public static event FiresecEventAggregator.ProgressDelegate Progress;
    }
}