using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Windows.Controls;

namespace Firesec
{
    public static class DispatcherFiresecClient
    {
        static Control control;

        delegate string StringDelegate();
        delegate string StringDelegateIntInt(int arg1, int arg2);

        static DispatcherFiresecClient()
        {
            control = new Control();
        }

        public static void Connect(string login, string password)
        {
            control.Dispatcher.Invoke(new Action<string, string>(NativeFiresecClient.Connect), login, password);
        }

        public static string GetCoreConfig()
        {
            return (string)control.Dispatcher.Invoke(new StringDelegate(NativeFiresecClient.GetCoreConfig));
        }

        public static string GetCoreState()
        {
            return (string)control.Dispatcher.Invoke(new StringDelegate(NativeFiresecClient.GetCoreState));
        }

        public static string GetMetaData()
        {
            return (string)control.Dispatcher.Invoke(new StringDelegate(NativeFiresecClient.GetMetaData));
        }

        public static string GetCoreDeviceParams()
        {
            return (string)control.Dispatcher.Invoke(new StringDelegate(NativeFiresecClient.GetCoreDeviceParams));
        }

        public static string ReadEvents(int fromId, int limit)
        {
            return (string)control.Dispatcher.Invoke(new StringDelegateIntInt(NativeFiresecClient.ReadEvents), fromId, limit);
        }

        public static void SetNewConfig(string coreConfig)
        {
            control.Dispatcher.Invoke(new Action<string>(NativeFiresecClient.SetNewConfig), coreConfig);
        }

        public static void DeviceWriteConfig(string coreConfig, string DevicePath)
        {
            control.Dispatcher.Invoke(new Action<string, string>(NativeFiresecClient.DeviceWriteConfig), coreConfig, DevicePath);
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
