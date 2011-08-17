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

        static DispatcherFiresecClient()
        {
            control = new Control();
        }

        public static bool Connect(string login, string password)
        {
            var result = (bool) control.Dispatcher.Invoke(new BoolDelegateStringString(NativeFiresecClient.Connect), login, password);
            return result;
        }

        public static void Disconnect()
        {
            control.Dispatcher.Invoke(new Action(NativeFiresecClient.Disconnect));
        }

        public static string GetCoreConfig()
        {
            return control.Dispatcher.Invoke(new StringDelegate(NativeFiresecClient.GetCoreConfig)) as string;
        }

        public static string GetCoreState()
        {
            return control.Dispatcher.Invoke(new StringDelegate(NativeFiresecClient.GetCoreState)) as string;
        }

        public static string GetMetaData()
        {
            return control.Dispatcher.Invoke(new StringDelegate(NativeFiresecClient.GetMetaData)) as string;
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
    }
}