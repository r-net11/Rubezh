using System;
using System.Diagnostics;
using System.Linq;
using Common;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using Microsoft.Win32;

namespace Infrastructure.Common
{
    public static class ServerLoadHelper
    {
        public static void SetLocation()
        {
            RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("software\\rubezh\\Firesec-2");
            registryKey.SetValue("FiresecServerPath", System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        public static string GetLocation()
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("software\\rubezh\\Firesec-2");
            if (registryKey != null)
            {
                var value = registryKey.GetValue("FiresecServerPath");
                if (value != null)
                {
                    if (File.Exists((string)value))
                        return (string)value;
                }
            }
            return null;
        }

        public static void SetStatus(FSServerState fsServerState)
        {
            RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("software\\rubezh\\Firesec-2");
            registryKey.SetValue("FiresecServiceState", (int)fsServerState);
        }

        public static FSServerState GetStatus()
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("software\\rubezh\\Firesec-2");
            if (registryKey != null)
            {
                var value = registryKey.GetValue("FiresecServiceState");
                if (value != null)
                {
                    return (FSServerState)value;
                }
            }

            return FSServerState.Closed;
        }

        public static bool WaitUntinlStarted()
        {
            for (int i = 0; i < 100; i++)
            {
                var fsAgentState = GetStatus();
                if (fsAgentState == FSServerState.Opened)
                    return true;
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            return false;
        }

        public static bool Load()
        {
#if DEBUG
            return true;
#endif

            Process[] processes = Process.GetProcessesByName("FiresecService");
            if (processes.Count() == 0)
            {
                try
                {
                    SetStatus(FSServerState.Closed);
                    if (!Start())
                        return false;
                    if (!WaitUntinlStarted())
                        return false;
                }
                catch (Exception e)
                {
                    Logger.Error(e, "ServerLoadHelper.Load");
                    return false;
                }
            }
            return true;
        }

        static bool Start()
        {
            var fileName = GetLocation();
            if (fileName != null)
            {
                fileName = @"..\FiresecService\FiresecService.exe";
            }

            if (!File.Exists(fileName))
            {
                Logger.Error("ServerLoadHelper.Start File Not Exist " + fileName);
                return false;
            }
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = fileName;
            process.Start();
            return true;
        }
    }

    public enum FSServerState
    {
        Closed = 0,
        Opening = 1,
        Opened = 2
    }
}